using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private EventInstance currentRhythmTrack;

    public delegate void OnMusicEvent(bool reset);
    public static event OnMusicEvent OnPlay;
    public static event OnMusicEvent OnResume;
    public static event OnMusicEvent OnPause;
    public static event OnMusicEvent OnStop;

    bool isPaused;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PlayRhythmSong(EventReference eventRef, bool interrupt = true)
    {
        if (interrupt)
            StopSong();

        currentRhythmTrack = RuntimeManager.CreateInstance(eventRef);

        currentRhythmTrack.start();

        isPaused = false;

        OnPlay?.Invoke(true);
    }

    public void PlaySfx(EventReference eventRef)
    {
        RuntimeManager.PlayOneShot(eventRef);
    }

    public void PauseSong()
    {
        if (!currentRhythmTrack.isValid())
            return;

        currentRhythmTrack.setPaused(true);

        isPaused = true;

        OnPause?.Invoke(false);
    }

    public void ResumeSong()
    {
        if (!currentRhythmTrack.isValid())
            return;

        currentRhythmTrack.setPaused(false);

        isPaused = false;

        OnResume?.Invoke(false);
    }

    public void StopSong()
    {
        if (!currentRhythmTrack.isValid())
            return;

        currentRhythmTrack.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        currentRhythmTrack.release();

        OnStop?.Invoke(true);
    }

    public bool TryGetCurrentRhythmTrack(out EventInstance track)
    {
        track = currentRhythmTrack;

        return currentRhythmTrack.isValid();
    }

    public float SongPositionSeconds()
    {
        if (!currentRhythmTrack.isValid())
            return 0f;

        currentRhythmTrack.getTimelinePosition(out int ms);

        return ms / 1000f;
    }

    public bool IsPlaying()
    {
        if (!currentRhythmTrack.isValid())
            return false;

        currentRhythmTrack.getPlaybackState(out PLAYBACK_STATE state);

        return state == PLAYBACK_STATE.PLAYING;
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    void OnDestroy()
    {
        if (currentRhythmTrack.isValid())
        {
            currentRhythmTrack.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            currentRhythmTrack.release();
        }
    }
}