using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class SoundSettings
{
    [Range(0f, 1f)]public float Volume;
    [Range(0.5f, 1f)]public float pitchMin;
    [Range(1f, 1.5f)]public float pitchMax;
}

[Serializable]
public class SongsData
{
    public AudioClip song;
    public int bpm = 120;
    public bool shouldLoop = true;
}

public class AudioManager : MonoBehaviour
{
    // Music
    public SoundSettings MusicSettings = new SoundSettings();
    
    [SerializeField] private AudioSource[] _audioSources = new AudioSource[2];
    private int sourceActive = 0;
    private AudioSource musicPlayer { get;set; }
    
    public double songDuration { get; private set; }
    public double beatDuration { get; private set; }
    public double dspSongStartTime { get;private set; }
    
    
    
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
     
    [SerializeField] public SongsData currentSongData { get; private set; }
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
    
    public void PlaySongData(SongsData songData, int delay=1)
    {
        if (songData == null || songData.song == null || songData.bpm <= 0)
        {
            Debug.LogError("Datos de canción inválidos");
            return;
        }

        double nextSongStartTime = AudioSettings.dspTime; //Now
        if(IsPlaying()) nextSongStartTime= dspSongStartTime + BeatManager.Instance.beatDuration*(BeatManager.Instance.counter+delay); // delayed on some Beats


        int nextSource = 1 - sourceActive;
        _audioSources[nextSource].clip = songData.song;
        _audioSources[nextSource].volume = MusicSettings.Volume;
        _audioSources[nextSource].loop = songData.shouldLoop;
        _audioSources[nextSource].PlayScheduled(nextSongStartTime);
        activeCoroutine = StartCoroutine(WaitForScheduledTime(songData,nextSongStartTime));
    }
    
    private IEnumerator WaitForScheduledTime(SongsData songData,double startTime)
    {
        while (AudioSettings.dspTime < startTime)
        {
            yield return null;
        }
        OnSongStarted(songData,startTime);
    }

    private void OnSongStarted(SongsData songData,double startTime)
    {
        /// logica de cambiar de audiosource y actualizar la info
        sourceActive = 1 -sourceActive ;
        musicPlayer = _audioSources[sourceActive];
        songDuration = (double)songData.song.samples / songData.song.frequency;
        dspSongStartTime = startTime;
        currentSongData = songData;
        beatDuration = (60d / songData.bpm);
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
        dspSongStartTime = AudioSettings.dspTime;
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
