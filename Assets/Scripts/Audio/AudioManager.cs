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
    public double beatDuration; // { get; private set; }
    public double dspSongStartTime; // { get;private set; }
    public List<double> cutFlags; // { get; private set; }

    public SongPlayingData(SongDataSO data, double startDSPTime)
    {
        songData = data;
        dspSongStartTime = startDSPTime;
        songDuration = (double)data.clip.samples / songData.clip.frequency;
        beatDuration = (60d / songData.bpm);
        cutFlags = new List<double>();
        foreach (int cut in data.cutFlags)
        {
            cutFlags.Add(cut*beatDuration + startDSPTime);
        }
        cutFlags.Add(songDuration + startDSPTime);
    }

    public void UpdateDataDSPTime(double dspNewTime)
    {
        /// To Implement
    }
}

public class AudioManager : MonoBehaviour
{
    // Music
    public SoundSettings MusicSettings = new SoundSettings();
    
    [SerializeField] private AudioSource[] _audioSources = new AudioSource[2];
    private int audioSourceActive = 0;
    private AudioSource musicPlayer { get;set; }
    [SerializeField] private List<SongDataSO> musicLibrary  = new List<SongDataSO>();
    //private Queue<SongPlayingData> songsPlayingQueue = new Queue<SongPlayingData>();
    [SerializeField] private List<SongPlayingData> songsPlayingQueue = new List<SongPlayingData>();
    public SongPlayingData currentSongPlaying =null;
    
    // Eventos por código
    public delegate void OnMusicEvent();
    public static event OnMusicEvent OnPlay;
    public static event OnMusicEvent OnResume;
    public static event OnMusicEvent OnPause;
    public static event OnMusicEvent OnStop;
    
    //Corrutina esperando activarse
    private Coroutine activeCoroutine;

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
            DontDestroyOnLoad(this.gameObject);
        }
    }
    
    
    public void PlaySong(string name, bool interrupt =false)
    {
        SongDataSO songData = musicLibrary.FirstOrDefault(x => x.name == name);
        if (songData != null)
        {
            if (interrupt) ForcePlaySong(songData); ///Salta la Queue y pone la cancion a sonar
                else QueueSongData(songData);
        }
    }

    public void PlaySong(int index, bool interrupt =false)
    {
        SongDataSO songData = null;
        if(index >= 0 && index < musicLibrary.Count) songData = musicLibrary[index];
        if (songData != null)
        {
            if (interrupt) ForcePlaySong(songData); ///Salta la Queue y pone la cancion a sonar
            else QueueSongData(songData);
        }
    }

    private void ForcePlaySong(SongDataSO songData)
    {
        double nextSongStartTime = AudioSettings.dspTime;
        SongPlayingData nextSongData = new SongPlayingData(songData,nextSongStartTime);
        songsPlayingQueue.Clear();
        songsPlayingQueue.Add(nextSongData);
        _audioSources[audioSourceActive].Stop();
        PrepareNextSong(songData,nextSongStartTime);
        activeCoroutine = StartCoroutine(WaitForScheduledTime(songData,nextSongStartTime));
    }
    
    private void QueueSongData(SongDataSO songData)
    {
        double nextSongStartTime = AudioSettings.dspTime; //Now
        // SET THE NewSong AT THE NEXT ENDFLAG OF THE CurrentSong <<<<
        if(songsPlayingQueue.Count>0) nextSongStartTime = GetNextEndPoint(); //El proximo endPoint  
        _audioSources[audioSourceActive].SetScheduledEndTime(nextSongStartTime);
        //NextSong
        SongPlayingData nextSongData = new SongPlayingData(songData,nextSongStartTime);
        songsPlayingQueue.Add(nextSongData);
        PrepareNextSong(songData,nextSongStartTime);
        activeCoroutine = StartCoroutine(WaitForScheduledTime(songData,nextSongStartTime));
    }

    private double GetNextEndPoint()
    {
        SongPlayingData songData = songsPlayingQueue[0];
        foreach (double cut in songData.cutFlags)
        {
            if( cut>= AudioSettings.dspTime) return cut;
        }
        return songData.songDuration+songData.dspSongStartTime;
    }
    private void PrepareNextSong(SongDataSO songData,double starTime)
    {
        int nextSource = 1 - audioSourceActive;
        _audioSources[nextSource].clip = songData.clip;
        ///_audioSources[nextSource].loop = songData.loopeable; NO SIRVE LOOPEAR ASI PORQUE AGREGA DELAY
        _audioSources[nextSource].volume = MusicSettings.Volume;
        _audioSources[nextSource].PlayScheduled(starTime);
    } 
    private IEnumerator WaitForScheduledTime(SongDataSO songData,double startTime)
    {
        while (AudioSettings.dspTime < startTime)
        {
            yield return null;
        }
        OnSongStarted(songData,startTime);
    } 
    private void OnSongStarted(SongDataSO songData,double startTime)
    {
        audioSourceActive = 1 - audioSourceActive ;
        musicPlayer = _audioSources[audioSourceActive]; 
        // if(songsPlayingQueue.Count>0){ if(songsPlayingQueue[0].songData.name != songData.songName) songsPlayingQueue.RemoveAt(0);}
        currentSongPlaying = songsPlayingQueue[0];//songsPlayingQueue.Dequeue();
        songsPlayingQueue.RemoveAt(0);
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
        songsPlayingQueue[0]?.UpdateDataDSPTime(AudioSettings.dspTime);
        musicPlayer.UnPause();
        OnPlay?.Invoke();
    }

    public void StopSong()
    {
        musicPlayer.Stop();
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
