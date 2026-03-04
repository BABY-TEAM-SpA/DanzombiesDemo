using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SoundSettings
{
    [Range(0f, 1f)]public float Volume;
    [Range(0.5f, 1f)]public float pitchMin;
    [Range(1f, 1.5f)]public float pitchMax;
}

[Serializable]
public class SongPlayingData
{
    public SongDataSO songData; // { get; private set; }
    public double songDuration; // { get; private set; }
    public double songDurationCompass;
    public double beatDuration; // { get; private set; }
    public double dspSongStartTime; // { get;private set; }
    public List<double> cutFlags = new List<double>(); // { get; private set; }
    
    public SongPlayingData(){}
    public SongPlayingData(SongDataSO data, double startDSPTime=0d)
    {
        songData = data;
        songDuration = (double)data.clip.samples / songData.clip.frequency;
        beatDuration = (60d / songData.bpm);
        songDurationCompass = (double) data.compassLong * data.metric* beatDuration;
        SetStartTime(startDSPTime);
    }
    public void SetStartTime(double startDSPTime)
    {
        dspSongStartTime = startDSPTime;
        cutFlags = new List<double>();
        foreach (int cut in songData.cutFlags)
        {
            cutFlags.Add(startDSPTime+(cut*beatDuration));
        }
        cutFlags.Add(startDSPTime+songDuration+songData.delayAtEnd);
    }

    public double GetNextCutFlag()
    {
        double now = AudioSettings.dspTime;
        foreach (double cut in cutFlags)
        {
            if (cut > now)
            {
                Debug.Log("CutPoint Finded");
                return cut;
            }
        }

        Debug.Log("Return Defaul");
        return now+0.1d;
    }
}

public class AudioManager : MonoBehaviour
{
    // Music
    public SoundSettings MusicSettings = new SoundSettings();

    [SerializeField] private AudioSource[] _audioSources = new AudioSource[2];
    [SerializeField] private int audioSourceActive = 1;
    private AudioSource musicPlayer { get;set; }
    [SerializeField] private List<SongDataSO> musicLibrary  = new List<SongDataSO>();
    //Corrutina esperando activarse
    private Coroutine nextSongToPlayCoroutine;
    private List<SongPlayingData> songsQueue = new List<SongPlayingData>(); //FIFO
    public SongPlayingData currentSongPlaying = new SongPlayingData();
    private double dspTime;
    
    // Eventos por código
    public delegate void OnMusicEvent();
    public static event OnMusicEvent OnPlay;
    public static event OnMusicEvent OnResume;
    public static event OnMusicEvent OnPause;
    public static event OnMusicEvent OnStop;
    
    // SFX
    public SoundSettings SFXsettings = new SoundSettings();
    public AudioSource SFXplayer;
    public AudioClip playerStepSFX;
    public AudioClip playerClapSFX;
    
    public static AudioManager Instance { get; private set; }
    private void Awake() 
    { 
        /// Esta es una de esas clases que van a estar tdo el tiempo activas
        /// Cuando hace sonar una cancion lanza un evento con la song data para que el BeatManager se active y haga beat
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this.gameObject); 
        } 
        else 
        { 
            Instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        currentSongPlaying = new SongPlayingData();
        musicPlayer = _audioSources[audioSourceActive];
    }

    public void OnEnable()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        dspTime = AudioSettings.dspTime;
        if (nextSongToPlayCoroutine == null)
        {
            if (musicPlayer.isPlaying)
            {
                if (songsQueue.Count == 0)
                {
                    if (currentSongPlaying.songData.loopeable &&
                        dspTime > currentSongPlaying.cutFlags.Last() - currentSongPlaying.beatDuration * 2)
                    {
                        Debug.Log("Queue SomeLoop");
                        QueueNextSongData(currentSongPlaying.songData);
                        //PlayClipInQueue();
                    }
                }
                else
                {
                    if (dspTime > currentSongPlaying.GetNextCutFlag() - currentSongPlaying.beatDuration * 2)
                    {
                        Debug.Log("CutFlagReached Playing NextSong");
                        PlayClipInQueue();
                    }
                }
            }
            else
            {
                if (songsQueue.Count > 0)
                {
                    Debug.Log("Playing Enqueue automatic");
                    PlayClipInQueue();
                }
            }
        }
    }

    public void PlaySong(string name, bool interrupt =false)
    {
        SongDataSO songData = musicLibrary.FirstOrDefault(x => x.name == name);
        if (songData != null) QueueNextSongData(songData, interrupt);
    }

    public void PlaySong(int index, bool interrupt =false)
    {
        SongDataSO songData = null;
        if(index >= 0 && index < musicLibrary.Count) songData = musicLibrary[index];
        if (songData != null) QueueNextSongData(songData, interrupt);
    }
    
    private void QueueNextSongData(SongDataSO songData, bool interrupt = false) 
    {
        if (interrupt) StopSong();
        SongPlayingData nextSongToPlay = new SongPlayingData(songData);
        songsQueue.Add(nextSongToPlay);
    }

    private void PlayClipInQueue()
    {
        SongPlayingData nextSongToPlay = songsQueue[0] ;
        double nextSongStartTime = AudioSettings.dspTime+0.1d; 
        if(currentSongPlaying !=null) nextSongStartTime = currentSongPlaying.GetNextCutFlag();
        nextSongToPlay.SetStartTime(nextSongStartTime);
        int nextSource = 1 - audioSourceActive;
        _audioSources[nextSource].clip = nextSongToPlay.songData.clip;
        _audioSources[nextSource].volume = MusicSettings.Volume;
        _audioSources[nextSource].PlayScheduled(nextSongStartTime);
        //_audioSources[audioSourceActive].SetScheduledEndTime(nextSongStartTime);
        nextSongToPlayCoroutine = StartCoroutine(WaitForScheduledTime(nextSongStartTime));
    }

    
    private IEnumerator WaitForScheduledTime(double startTime)
    {
        while (AudioSettings.dspTime < startTime)
        {
            yield return null; 
        }
        OnSongStarted();
        
    } 
    private void OnSongStarted()
    {
        nextSongToPlayCoroutine = null;
        //currentSongPlaying = songsQueue.Dequeue();
        currentSongPlaying = songsQueue[0];
        songsQueue.RemoveAt(0);
        //Debug.Log("Playing:"+ currentSongPlaying.songData.name + " Remaing in Queue:" + songsQueue.Count.ToString());
        audioSourceActive = 1 - audioSourceActive ;
        musicPlayer = _audioSources[audioSourceActive];
        OnPlay?.Invoke();
    }
    
    public bool IsPlaying()
    {
        return (musicPlayer!=null)?musicPlayer.isPlaying:false;
    }
    public void PauseSong()
    {
        musicPlayer.Pause();
        OnPause?.Invoke();
    }

    public void ResumeSong()
    {
        //currentSongPlaying.UpdateDataDSPTime(AudioSettings.dspTime);
        musicPlayer.UnPause();
        OnPlay?.Invoke();
    }

    public void StopSong()
    {
        if (nextSongToPlayCoroutine != null)
        {
            //Debug.Log("Killed Coroutine");
            StopCoroutine(nextSongToPlayCoroutine);
            nextSongToPlayCoroutine = null;
        }
        songsQueue.Clear();
        currentSongPlaying = null;
        foreach(AudioSource audioSource in _audioSources) audioSource.Stop();
        OnStop?.Invoke();
    }
    public void OnPauseEvent(bool pause)
    {
        if (pause)
        {
            PauseSong();
        }
        else
        {
            ResumeSong();
        }
    }
    
    
    
}
