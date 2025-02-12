using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum BeatType
{
    Negra,
    Blanca,
    Redonda
}

[Serializable]
public class SongsData
{
    public AudioClip song;
    public int bpm;
}

public class BeatManager : MonoBehaviour
{
    
    public AudioSource _audioSource;
    
    public delegate void OnHalfBeatEvent();
    public static event OnHalfBeatEvent OnHalfBeat;
    //public delegate void OnBeatEvent(BeatType type);
    public delegate void OnPlayEvent(float speed);
    public static event OnPlayEvent OnPlay;
    public delegate void OnBeatEvent();
    public static event OnBeatEvent OnPreBeat;
    public static event OnBeatEvent OnBeat;
    public static event OnBeatEvent OnPostBeat;
    
    
    [Range(0f, 1f)] public float margen;
    public bool onMargen;

    public SongsData songData {get; private set;}
    [SerializeField] private List<SongsData> allSongs = new List<SongsData>();

    public float beatDuration { get; private set; }
    private float timer;
    private int counter;
    private int totalcounter;
    
    //auxiliares
    private bool canHalfBeat;
    private bool canPre;
    private bool canBeat;
    private bool canPost;
    
    public static BeatManager Instance { get; private set; }
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this.gameObject); 
        } 
        else 
        { 
            Instance = this;
            //DontDestroyOnLoad(this);
        } 
    }
    
    private void OnEnable()
    {
        //LevelController.OnPauseEvent += OnPauseEventReceiver;
        //LevelController.OnPlayEvent += OnPlayingEventReceiver;
    }

    private void OnDisable()
    {
        
        //LevelController.OnPauseEvent -= OnPauseEventReceiver;
        //LevelController.OnPlayEvent -= OnPlayingEventReceiver;
    }

    private void Start()
    {
        _audioSource = this.GetComponent<AudioSource>();
        
    }

    private void OnPauseEventReceiver(bool isPaused)
    {
        if (isPaused)
        {
            PauseSong();
        }
        else
        {
            ResumeSong();
        }
    }
    private void OnPlayingEventReceiver(bool isPlaying)
    {
        if (isPlaying)
        {
            PlaySong();
        }
        else
        {
            StopSong();
        }
    }

    void Update()
    {
        if (_audioSource.isPlaying)
        {
            float songTime = _audioSource.time;
            
            if((songTime >= (beatDuration * counter)- beatDuration / 2) && canHalfBeat)
            {
                canHalfBeat = false;
                HalfBeat();
                canPre = true;
            }
            
            else if (songTime >= ((beatDuration * counter) - beatDuration * margen)&& canPre)
            {
                canPre = false;
                PreBeat();
                canBeat = true;
            }
            else if (songTime >= beatDuration * counter && canBeat)
            {
                canBeat = false;
                timer = songTime;
                HalfBeat();
                Beat();
                canPost = true;
            }
            
            else if (songTime >= ((beatDuration * counter) + beatDuration * margen) && canPost)
            {
                canPost = false;
                PostBeat();
                canHalfBeat = true;
            }
        }
    }

    void HalfBeat()
    {
        if (OnHalfBeat != null)
        {
            OnHalfBeat();
        }
    }
    
    void PreBeat()
    {
        onMargen = true;
        if (OnPreBeat != null)
        {
            OnPreBeat();
            /*OnPreBeat(BeatType.Negra);
            if (counter%2==0)
            {
                OnPreBeat(BeatType.Blanca);
                if (counter%4==0)
                {
                    OnPreBeat(BeatType.Redonda);
                }
            }*/

        }
        
    }
    void Beat()
    {
        
        if (OnBeat != null)
        {
            OnBeat();
            /*OnBeat(BeatType.Negra);
            if (counter%2==0)
            {
                OnBeat(BeatType.Blanca);
                if (counter%4==0)
                {
                    OnBeat(BeatType.Redonda);
                }
            }*/
            
            
        }
        
    }
    void PostBeat()
    {
        onMargen = false;
        counter += 1;
        if (OnPostBeat != null)
        {
            OnPostBeat(); 
            /*OnPostBeat(BeatType.Negra);
            if (counter%2==0)
            {
                OnPostBeat(BeatType.Blanca);
                if (counter%4==0)
                {
                    OnPostBeat(BeatType.Redonda);
                }
            }*/
            
        }
        
    }

    public void PlaySong()
    {
        
        onMargen = false;
        
        canHalfBeat = true;
        canPre = false;
        canBeat = false;
        canPost = false;
        songData = GetRandomSong();
        timer =0; 
        counter=0;
        totalcounter=0;
        _audioSource.clip = songData.song;
        beatDuration = (60f / songData.bpm); // negras por seg.... duracion de una negra...
        timer = beatDuration;

        if(OnPlay != null)
        {
            OnPlay(beatDuration);
        }
        _audioSource.Play();
    }

    public SongsData GetRandomSong()
    {
        return allSongs[Random.Range(0, allSongs.Count)];
    }

    public void PauseSong()
    {
        _audioSource.Pause();
    }
    public void ResumeSong()
    {
        _audioSource.Play();
    }

    public void StopSong()
    {
        _audioSource.Stop();
    }
}
