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
    public bool isLoopVersion;

    public SongPlayingData(SongDataSO data, double startDSPTime,bool ILP)
    {
        songData = data;
        isLoopVersion = ILP;
        dspSongStartTime = startDSPTime;
        songDuration = (double)data.clip.samples / songData.clip.frequency;
        beatDuration = (60d / songData.bpm);
        cutFlags = new List<double>();
        foreach (int cut in data.cutFlags)
        {
            cutFlags.Add(cut*beatDuration);
        }
        cutFlags.Add(songDuration);
    }
}

public class AudioManager : MonoBehaviour
{
    // Music
    public SoundSettings MusicSettings = new SoundSettings();

    [SerializeField] private AudioSource[] _audioSources = new AudioSource[2];
    [SerializeField] private int audioSourceActive = 1;
    private AudioSource musicPlayer { get;set; }
    [SerializeField] private double machineDelay = 0.054d;
    [SerializeField] private List<SongDataSO> musicLibrary  = new List<SongDataSO>();
    //Corrutina esperando activarse
    private Coroutine nextSongToPlayCoroutine;
    [SerializeField] private List<SongPlayingData> nextSongsToPlay = new List<SongPlayingData>();
    //private SongPlayingData nextSongToPlay = null;
    public SongPlayingData currentSongPlaying =null;
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
            DontDestroyOnLoad(this.gameObject);
        }
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
    
    private void QueueNextSongData(SongDataSO songData, bool interrupt = false,bool isLoopVersion=false) 
    {
        
        double nextSongStartTime = interrupt? AudioSettings.dspTime:GetNextEndPoint(isLoopVersion);
        SongPlayingData nextSongToPlay = new SongPlayingData(songData,nextSongStartTime,isLoopVersion);
        
        if (interrupt)
        {
            if (nextSongToPlayCoroutine != null)
            {
                Debug.Log("Killed Coroutine");
                StopCoroutine(nextSongToPlayCoroutine);
                nextSongToPlayCoroutine = null;
            }
            nextSongsToPlay.Clear();
            nextSongsToPlay.Add(nextSongToPlay);
            foreach(AudioSource audioSource in _audioSources) audioSource.Stop();
            int nextSource = 1 - audioSourceActive;
            _audioSources[nextSource].clip = songData.clip;
            _audioSources[nextSource].volume = MusicSettings.Volume;
            _audioSources[nextSource].Play();//.PlayScheduled(nextSongStartTime);
            OnSongStarted();
        }
        else
        {
            nextSongsToPlay.Add(nextSongToPlay);
            int nextSource = 1 - audioSourceActive;
            _audioSources[nextSource].clip = songData.clip;
            _audioSources[nextSource].volume = MusicSettings.Volume;
            _audioSources[nextSource].PlayScheduled(nextSongStartTime);
            _audioSources[audioSourceActive].SetScheduledEndTime(nextSongStartTime);
            nextSongToPlayCoroutine = StartCoroutine(WaitForScheduledTime(songData,nextSongStartTime));
            Debug.Log("Started Coroutine");
        }
    }

    private double GetNextEndPoint(bool forceLastCut=false) //Obtener el proximo EndPoint de la cancion 
    {
        SongPlayingData songData = currentSongPlaying;
        if(nextSongsToPlay.Count > 0) songData = nextSongsToPlay.Last().isLoopVersion?currentSongPlaying:nextSongsToPlay.Last();
        if (songData != null)
        {
            if(forceLastCut) return songData.cutFlags.Last()+songData.dspSongStartTime-machineDelay;
            foreach (double cut in songData.cutFlags)
            {
                if( cut+songData.dspSongStartTime>= AudioSettings.dspTime) return cut+songData.dspSongStartTime-machineDelay;
            }
        }
        return AudioSettings.dspTime; //Now -> Si todos los cutTimes pasaron (eso incluye el final de la cancion)
    }
    private IEnumerator WaitForScheduledTime(SongDataSO songData,double startTime)
    {
        while (AudioSettings.dspTime < startTime)
        {
            yield return null; 
        }
        Debug.Log("Ended Coroutine");
        Debug.Log("Playing: "+songData.clip.name);
        OnSongStarted();
    } 
    private void OnSongStarted()
    {
        currentSongPlaying = nextSongsToPlay.First();
        nextSongsToPlay.RemoveAt(0);
        nextSongToPlayCoroutine = null;
        audioSourceActive = 1 - audioSourceActive ;
        musicPlayer = _audioSources[audioSourceActive];
        
        if (currentSongPlaying.songData.loopeable && nextSongsToPlay.Count == 0)
        {
            Debug.Log("Loopeable");
            QueueNextSongData(currentSongPlaying.songData,false,true);
            
        }
        
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
