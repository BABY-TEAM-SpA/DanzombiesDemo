using System;
using System.Runtime.InteropServices;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class AudioManager : Service<AudioManager>
{
    #region [VARIABLES]
    private EventInstance currentRhythmTrack;

    public delegate void OnMusicEvent(bool reset);
    public static event OnMusicEvent OnResume;
    public static event OnMusicEvent OnPause;
    public static event OnMusicEvent OnStop;

    bool isPaused;
    #endregion

    #region [UNITY]
    protected override void OnDestroy()
    {
        if (currentRhythmTrack.isValid())
        {
            currentRhythmTrack.stop(STOP_MODE.IMMEDIATE);
            currentRhythmTrack.release();
        }

        base.OnDestroy();
    }
    #endregion

    #region [METHODS]
    #region API - Music Player Handling
    public void PlayRhythmSong(EventReference eventRef, bool interrupt = true)
    {
        if (interrupt)
            StopSong();
        currentRhythmTrack = RuntimeManager.CreateInstance(eventRef);
        isPaused = false;
        currentRhythmTrack.setCallback(
            TimelineCallback,
            EVENT_CALLBACK_TYPE.TIMELINE_BEAT
        );

        currentRhythmTrack.start();
    }

    public void PauseSong()
    {
        if (!currentRhythmTrack.isValid())
            return;
        isPaused = true;
        currentRhythmTrack.setPaused(isPaused);
        OnPause?.Invoke(false);
    }

    public void ResumeSong()
    {
        if (!currentRhythmTrack.isValid())
            return;
        isPaused = false;
        currentRhythmTrack.setPaused(isPaused);
        OnResume?.Invoke(false);
    }

    public void StopSong()
    {
        if (!currentRhythmTrack.isValid())
            return;
        
        currentRhythmTrack.stop(STOP_MODE.IMMEDIATE);
        currentRhythmTrack.release();
        OnStop?.Invoke(true);
    }
    #endregion

    #region API - Music Player Info
    public bool TryGetCurrentRhythmTrack(out EventInstance track)
    {
        track = currentRhythmTrack;
        return currentRhythmTrack.isValid();
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

    public float SongPositionSeconds()
    {
        if (!currentRhythmTrack.isValid())
            return 0f;

        currentRhythmTrack.getTimelinePosition(out int ms);

        return ms / 1000f;
    }

    public float SongPositionSecondsRelativeToCurrentRhythm()
    {
        if (!currentRhythmTrack.isValid())
            return 0f;
        currentRhythmTrack.getTimelinePosition(out int ms);
        return Mathf.RoundToInt(ms / 1000f);
    }
    #endregion

    #region Helpers
    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    private static FMOD.RESULT TimelineCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        if (type != EVENT_CALLBACK_TYPE.TIMELINE_BEAT) return FMOD.RESULT.OK;

        TIMELINE_BEAT_PROPERTIES beat = Marshal.PtrToStructure<TIMELINE_BEAT_PROPERTIES>(parameterPtr);
        BeatManager.Instance?.HandleBeat(beat.bar, beat.beat, beat.tempo, beat.timesignatureupper, beat.timesignaturelower, beat.position);
        return FMOD.RESULT.OK;
    }
    #endregion
    #endregion
}