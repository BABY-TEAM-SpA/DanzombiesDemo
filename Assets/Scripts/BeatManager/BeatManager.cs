using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public bool shouldLoop;
    public int  Metrica = 4; // Tempo: m/4
}

public class BeatManager : MonoBehaviour
{
    
    //public List<AudioSourceData> _audioSources = new List<AudioSourceData>();
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] public SongsData currenSongData{ get; private set;}
    
    public bool nextSongMustPlay;
    
    public delegate void OnMusicEvent(float speed);
    public static event OnMusicEvent OnPlay;
    public static event OnMusicEvent OnPause;
    public static event OnMusicEvent OnStop;
    
    public delegate void OnBeatEvent(int metrica);
    public static event OnBeatEvent OnPreBeat;
    public static event OnBeatEvent OnBeat;
    public static event OnBeatEvent OnPostBeat;
    
    [SerializeField][Range(0f, 1f)] public float margen { get;private set; }=0.25f;
    public bool onMargen { get;private set;}
    public float beatDuration { get; private set; }
    private int counter;
    
    //auxiliares
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
    
    void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    void Update()
    {
        if (_audioSource.isPlaying)
        {
            float songTime = _audioSource.time;
            
            if (songTime >= ((beatDuration * counter) - beatDuration * margen)&& canPre)
            {
                PreBeat();
            }
            else if (songTime >= beatDuration * counter && canBeat)
            {
                Beat();
            }
            else if (songTime >= ((beatDuration * counter) + beatDuration * margen) && canPost)
            {
                PostBeat();
            }
        }
        else
        {
            OnSongEndedOrStoped();
        }
    }
    
    void PreBeat()
    {
        canPre = false;
        onMargen = true;
        OnPreBeat?.Invoke(counter % currenSongData.Metrica);
        canBeat = true;
    }
    void Beat()
    {
        canBeat = false;
        OnBeat?.Invoke(counter % currenSongData.Metrica);
        canPost = true;
    }
    void PostBeat()
    {
        onMargen = false;
        OnPostBeat?.Invoke(counter % currenSongData.Metrica);
        counter += 1;
        canPost = false;
        canPre = true;
    }

    public void PlaySongData(SongsData songData)
    {
        SetAudioSource(songData);
        onMargen = false;
        canPre = false;
        canBeat = true;
        canPost = false;
        if (_audioSource.clip != null)_audioSource.Play();
        OnPlay?.Invoke(beatDuration);
    }
    
    public void PauseSong()
    {
        _audioSource.Pause();
        OnPause?.Invoke(beatDuration);
    }
    public void ResumeSong()
    {
        _audioSource.Play();
        OnPlay?.Invoke(beatDuration);
    }

    public void StopSong()
    {
        _audioSource.Stop();
        OnStop?.Invoke(beatDuration); 
    }

    private void OnSongEndedOrStoped()
    {
        Debug.Log("OnSongEndedOrStoped");
        OnStop?.Invoke(beatDuration);
    }

    private void SetAudioSource(SongsData data)
    {
        currenSongData = data;
        _audioSource.clip = data.song;
        _audioSource.loop = data.shouldLoop;
        beatDuration = (60f / currenSongData.bpm); // negras por seg.... duracion de una negra...
        counter=0;
    }
    
}
