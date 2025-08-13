using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    public int bpm = 120;
    public bool shouldLoop = true;
    public int Metrica = 4; // m/4
}

public class BeatManager : MonoBehaviour
{
    [Header("Audio")] 
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] public SongsData currentSongData { get; private set; }
    
    private Queue<SongsData> playlist = new Queue<SongsData>();

    [Header("Sincronización")]
    [Range(0f, 0.5f)] public float margen = 0.25f;
    public int subdivisiones = 1; // 1 = negras, 2 = corcheas, etc.
    public bool onMargen { get; private set; }
    public float beatDuration { get; private set; }
    public int metrica { get; private set; }

    [Header("Eventos por Inspector")]
    public UnityEvent<int,int> onPreBeatInspector;
    public UnityEvent<int,int> onBeatInspector;
    public UnityEvent<int,int> onPostBeatInspector;

    // Eventos por código
    public delegate void OnMusicEvent(float speed);
    public static event OnMusicEvent OnPlay;
    public static event OnMusicEvent OnPause;
    public static event OnMusicEvent OnStop;

    public delegate void OnBeatEvent(int counter, int metricaCounter);
    public static event OnBeatEvent OnPreBeat;
    public static event OnBeatEvent OnBeat;
    public static event OnBeatEvent OnPostBeat;

    // Estado interno
    private int counter;
    private bool canPre;
    private bool canBeat;
    private bool canPost;

    // DSP timing
    private double dspSongStartTime;
    private float songTime;
    private float lastSongTime;
    public static BeatManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (_audioSource.isPlaying)
        {
            songTime = (float)(AudioSettings.dspTime - dspSongStartTime);
            if (songTime < lastSongTime)
            {
                counter = 0;
                canPre = false;
                canBeat = true;
                canPost = false;
            }
            lastSongTime = songTime;

            // Comprobación de eventos
            if (songTime >= ((beatDuration * counter) - beatDuration * margen) && canPre)
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
            OnSongEndedOrStopped();
        }
    }

    void PreBeat()
    {
        canPre = false;
        onMargen = true;

        if(counter!=0)OnPreBeat?.Invoke((counter-1), (counter-1)%metrica);
        if(counter!=0)onPreBeatInspector?.Invoke((counter-1), (counter-1)%metrica);

        canBeat = true;
    }

    void Beat()
    {
        canBeat = false;
        Debug.Log(counter);
        if(counter!=0)OnBeat?.Invoke((counter-1),(counter-1)%metrica);
        if(counter!=0)onBeatInspector?.Invoke((counter-1), (counter-1)%metrica);

        canPost = true;
    }

    void PostBeat()
    {
        onMargen = false;

        if(counter!=0)OnPostBeat?.Invoke((counter-1),(counter-1)% metrica);
        if(counter!=0)onPostBeatInspector?.Invoke((counter-1), (counter-1) % metrica);

        counter += 1;
        canPost = false;
        canPre = true;
    }

    public void PlaySongData(SongsData songData)
    {
        if (songData == null || songData.song == null || songData.bpm <= 0)
        {
            Debug.LogError("Datos de canción inválidos");
            return;
        }

        SetAudioSource(songData);
        dspSongStartTime = AudioSettings.dspTime;
        onMargen = false;
        canPre = false;
        canBeat = true;
        canPost = false;

        _audioSource.PlayScheduled(dspSongStartTime);

        OnPlay?.Invoke(beatDuration);
    }

    public void PauseSong()
    {
        _audioSource.Pause();
        OnPause?.Invoke(beatDuration);
    }

    public void ResumeSong()
    {
        dspSongStartTime = AudioSettings.dspTime - songTime;
        _audioSource.Play();
        OnPlay?.Invoke(beatDuration);
    }

    public void StopSong()
    {
        _audioSource.Stop();
        OnStop?.Invoke(beatDuration);
    }

    private void OnSongEndedOrStopped()
    {
        if (playlist.Count > 0)
        {
            PlaySongData(playlist.Dequeue());
        }
        else
        {
            OnStop?.Invoke(beatDuration);
        }
    }

    private void SetAudioSource(SongsData data)
    {
        currentSongData = data;
        _audioSource.clip = data.song;
        _audioSource.loop = data.shouldLoop;
        beatDuration = (60f / currentSongData.bpm) / subdivisiones;
        metrica = currentSongData.Metrica;
        counter = 0;
    }

    public void AddToPlaylist(SongsData song)
    {
        if (song != null) playlist.Enqueue(song);
    }

    public bool IsPlaying()
    {
        return _audioSource.isPlaying;
    }
}