using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SoundSettings
{
    [Range(0f, 1f)] public float Volume = 1f;
    [Range(0.5f, 1f)] public float pitchMin = 1f;
    [Range(1f, 1.5f)] public float pitchMax = 1f;
}

[Serializable]
public class SongPlayingData
{
    public SongDataSO songData;

    public double songDuration;
    public double songDurationCompass;
    public double beatDuration;
    public double dspSongStartTime;

    public List<double> cutFlags = new List<double>();

    private int nextCutIndex;

    public SongPlayingData(){}

    public SongPlayingData(SongDataSO data)
    {
        songData = data;

        songDuration = data.clip.length;

        beatDuration = 60d / data.bpm;

        songDurationCompass = data.compassLong * data.metric * beatDuration;
    }

    public void SetStartTime(double startDSPTime)
    {
        dspSongStartTime = startDSPTime;

        cutFlags.Clear();
        nextCutIndex = 0;

        foreach (int cut in songData.cutFlags)
        {
            cutFlags.Add(startDSPTime + (cut * beatDuration));
        }

        cutFlags.Add(startDSPTime + songDuration + songData.delayAtEnd);
    }

    public double GetNextCutFlag()
    {
        double now = AudioSettings.dspTime;

        while (nextCutIndex < cutFlags.Count && cutFlags[nextCutIndex] <= now)
            nextCutIndex++;

        if (nextCutIndex < cutFlags.Count)
            return cutFlags[nextCutIndex];

        return now + 0.1d;
    }
}

public class AudioManager : MonoBehaviour
{
    public SoundSettings MusicSettings = new SoundSettings();

    [SerializeField] private AudioSource[] _audioSources = new AudioSource[2];

    private int audioSourceActive = 0;
    private AudioSource musicPlayer;

    [SerializeField] private List<SongDataSO> musicLibrary = new List<SongDataSO>();

    private Coroutine nextSongCoroutine;

    private readonly List<SongPlayingData> songsQueue = new List<SongPlayingData>();

    public SongPlayingData currentSongPlaying;

    public delegate void OnMusicEvent();
    public static event OnMusicEvent OnPlay;
    public static event OnMusicEvent OnResume;
    public static event OnMusicEvent OnPause;
    public static event OnMusicEvent OnStop;

    public SoundSettings SFXsettings = new SoundSettings();
    public AudioSource SFXplayer;

    public AudioClip playerStepSFX;
    public AudioClip playerClapSFX;

    private double pauseDSPTime;
    private double pauseOffset;

    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);
        if (_audioSources == null || _audioSources.Length < 2)
        {
            Debug.LogError("AudioManager requires two AudioSources.");
            return;
        }

        musicPlayer = _audioSources[audioSourceActive];
    }

    void Update()
    {
        if (nextSongCoroutine != null)
            return;

        if (!IsPlaying())
        {
            if (songsQueue.Count > 0)
                PlayClipInQueue();

            return;
        }

        if (songsQueue.Count == 0)
        {
            if (currentSongPlaying != null &&
                currentSongPlaying.songData.loopeable &&
                currentSongPlaying.cutFlags.Count > 0)
            {
                double dspTime = AudioSettings.dspTime;

                if (dspTime >
                    currentSongPlaying.cutFlags.Last() -
                    currentSongPlaying.beatDuration * 2)
                {
                    QueueNextSongData(currentSongPlaying.songData);
                }
            }

            return;
        }

        double now = AudioSettings.dspTime;

        if (currentSongPlaying != null &&
            now >
            currentSongPlaying.GetNextCutFlag() -
            currentSongPlaying.beatDuration * 2)
        {
            PlayClipInQueue();
        }
    }

    public void PlaySong(string name, bool interrupt = false)
    {
        SongDataSO songData =
            musicLibrary.FirstOrDefault(x => x.name == name);

        if (songData != null)
            QueueNextSongData(songData, interrupt);
    }

    public void PlaySong(int index, bool interrupt = false)
    {
        if (index < 0 || index >= musicLibrary.Count)
            return;

        QueueNextSongData(musicLibrary[index], interrupt);
    }

    private void QueueNextSongData(SongDataSO songData, bool interrupt = false)
    {
        if (songData == null)
            return;

        if (interrupt)
            StopSong();

        if (songsQueue.Count > 0 &&
            songsQueue.Last().songData == songData)
            return;

        SongPlayingData nextSong = new SongPlayingData(songData);

        songsQueue.Add(nextSong);
    }

    private void PlayClipInQueue()
    {
        if (songsQueue.Count == 0)
            return;

        SongPlayingData nextSong = songsQueue[0];

        double startTime;

        if (currentSongPlaying == null)
            startTime = AudioSettings.dspTime + 0.2d;
        else
            startTime = currentSongPlaying.GetNextCutFlag();

        nextSong.SetStartTime(startTime);

        int nextSource = 1 - audioSourceActive;

        AudioSource source = _audioSources[nextSource];

        source.clip = nextSong.songData.clip;
        source.volume = MusicSettings.Volume;

        source.PlayScheduled(startTime);

        nextSongCoroutine =
            StartCoroutine(WaitForScheduledTime(startTime));
    }

    private IEnumerator WaitForScheduledTime(double startTime)
    {
        while (AudioSettings.dspTime < startTime)
            yield return null;

        OnSongStarted();
    }

    private void OnSongStarted()
    {
        nextSongCoroutine = null;

        currentSongPlaying = songsQueue[0];

        songsQueue.RemoveAt(0);

        audioSourceActive = 1 - audioSourceActive;

        musicPlayer = _audioSources[audioSourceActive];

        OnPlay?.Invoke();
    }

    public bool IsPlaying()
    {
        return musicPlayer != null && musicPlayer.isPlaying;
    }

    public void PauseSong()
    {
        if (musicPlayer == null || !musicPlayer.isPlaying)
            return;

        pauseDSPTime = AudioSettings.dspTime;

        musicPlayer.Pause();

        OnPause?.Invoke();
    }

    public void ResumeSong()
    {
        if (musicPlayer == null)
            return;

        pauseOffset += AudioSettings.dspTime - pauseDSPTime;

        musicPlayer.UnPause();

        OnResume?.Invoke();
    }

    public void StopSong()
    {
        if (nextSongCoroutine != null)
        {
            StopCoroutine(nextSongCoroutine);
            nextSongCoroutine = null;
        }

        songsQueue.Clear();

        currentSongPlaying = null;

        foreach (AudioSource source in _audioSources)
        {
            if (source != null)
                source.Stop();
        }

        OnStop?.Invoke();
    }

    public void OnPauseEvent(bool pause)
    {
        if (pause)
            PauseSong();
        else
            ResumeSong();
    }

    public double SongPositionDSP()
    {
        if (currentSongPlaying == null)
            return 0;

        return AudioSettings.dspTime - currentSongPlaying.dspSongStartTime - pauseOffset;
    }

    public int SongPositionBeats()
    {
        if (currentSongPlaying == null)
            return 0;
        int value = Mathf.FloorToInt((float)(SongPositionDSP()  / currentSongPlaying.beatDuration));
        return value<0 ? 0 : value;
    }
}