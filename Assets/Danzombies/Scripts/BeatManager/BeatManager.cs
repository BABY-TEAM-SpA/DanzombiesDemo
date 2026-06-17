using System;
using System.Runtime.InteropServices;
using FMOD.Studio;
using UnityEngine;


public class BeatManager : MonoBehaviour
{
    public enum BeatType
    {
        FullBeat,
        HalfBeat
    }

    [Header("Sync")]
    [Range(0f, 0.4f)]
    public double margenPercentOnBeat = 0.25d;

    [Range(0f, 1f)]
    public double greatPercentOnMargin = 0.5d;

    [Range(0f, 0.5f)]
    public double perfectPercentOnMargin = 0.1d;

    public double quarterBeatDuration { get; private set; } = 1f;
    public double eighthBeatDuration { get; private set; } = 0.5f;

    public int localCounterNegra { get; private set; } = 1;
    public int globalCounterNegra { get; private set; } = 1;
    public int globalCounterCorchea { get; private set; } = 1;

    public delegate void OnUpdate(double beatDuration);
    public static event OnUpdate OnUpdateEvent;
    double songTime;

    double lastBeatTime;
    double nextBeatTime;
    
    double lastHalfBeatTime;
    double nextHalfBeatTime;

    private bool preTrigger;
    private bool beatTrigger;
    private bool postTrigger;
    
    private bool halfPreTrigger;
    private bool halfBeatTrigger;
    private bool halfPostTrigger;
    
    public delegate void OnBeatEvent(int counter, BeatType beatType);
    public static event OnBeatEvent OnPreBeat;
    public static event OnBeatEvent OnBeat;
    public static event OnBeatEvent OnPostBeat;

    public static BeatManager Instance { get; private set; }

    EventInstance trackedMusic;
    static BeatManager callbackTarget;

    void Awake()
    {
        Instance = this;
        callbackTarget = this;
    }

    void OnEnable()
    {
        AudioManager.OnPlay += OnPlayEvent;
    }

    void OnDisable()
    {
        AudioManager.OnPlay -= OnPlayEvent;
    }

    void Update()
    {
        if (!AudioManager.Instance.IsPlaying()) return;
        songTime = AudioManager.Instance.SongPositionSeconds();
        HandlePrePostBeat();
        HandleHalfPrePostBeat();
    }

    void OnPlayEvent(bool resetCounter)
    {
        ResetBeatManager(resetCounter);
        if (AudioManager.Instance.TryGetCurrentRhythmTrack(out trackedMusic)) 
            trackedMusic.setCallback( TimelineCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        OnUpdateEvent?.Invoke(quarterBeatDuration);
        
    }
    
    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    static FMOD.RESULT TimelineCallback(EVENT_CALLBACK_TYPE type,IntPtr instancePtr,IntPtr parameterPtr)
    {
        if (type != EVENT_CALLBACK_TYPE.TIMELINE_BEAT) return FMOD.RESULT.OK;
        TIMELINE_BEAT_PROPERTIES beat = Marshal.PtrToStructure<TIMELINE_BEAT_PROPERTIES>(parameterPtr);
        callbackTarget?.HandleBeat( beat.bar, beat.beat, beat.tempo);
        return FMOD.RESULT.OK;
    }
    
    void HandleBeat(int bar, int beat, float tempo)
    {
        lastBeatTime = AudioManager.Instance.SongPositionSeconds();
        quarterBeatDuration = 60d / tempo;
        nextBeatTime = lastBeatTime + quarterBeatDuration;
        preTrigger = true;
        beatTrigger = true;
        postTrigger = false;
        globalCounterNegra++;
        OnBeat?.Invoke(globalCounterNegra, BeatType.FullBeat);
        eighthBeatDuration = quarterBeatDuration * 0.5d;
        HalfBeat(lastBeatTime);
    }
    
    void HalfBeat(double beatTime)
    {
        lastHalfBeatTime = beatTime;
        nextHalfBeatTime = lastHalfBeatTime + eighthBeatDuration;
        halfPreTrigger = true;
        halfBeatTrigger = true;
        halfPostTrigger = false;
        globalCounterCorchea++;
        OnBeat?.Invoke(globalCounterNegra, BeatType.HalfBeat);
    }
    
    void HandlePrePostBeat()
    {
        double margin = quarterBeatDuration * margenPercentOnBeat;
        
        //preBeat
        if (!preTrigger  && songTime >= nextBeatTime - margin)
        {
            preTrigger = true;
            beatTrigger = false;
            postTrigger = true;
            OnPreBeat?.Invoke(globalCounterNegra, BeatType.FullBeat);
        }
        
        //beat
            //HandleBeat()
        
        //postBeat
        if (!postTrigger && beatTrigger && songTime >= lastBeatTime + margin)
        {
            preTrigger = false;
            beatTrigger = true;
            postTrigger = true;
            OnPostBeat?.Invoke(globalCounterNegra, BeatType.FullBeat);
        }
    }
    void HandleHalfPrePostBeat()
    {
        double margin = eighthBeatDuration * margenPercentOnBeat;
        
        //preBeat
        if (!halfPreTrigger && songTime >= nextHalfBeatTime - margin)
        {
            halfPreTrigger = true;
            halfBeatTrigger = false;
            halfPostTrigger = true;
            OnPreBeat?.Invoke(globalCounterCorchea, BeatType.HalfBeat);
        }

        if (halfPreTrigger && !halfBeatTrigger && songTime > nextHalfBeatTime)
        {
            HalfBeat(AudioManager.Instance.SongPositionSeconds());
        }
        
        //postBeat
        if (!halfPostTrigger && halfBeatTrigger && songTime >= lastHalfBeatTime + margin)
        {
            halfPreTrigger = false;
            halfBeatTrigger = true;
            halfPostTrigger = true;
            OnPostBeat?.Invoke(globalCounterCorchea, BeatType.HalfBeat);
        }
    }
    
    public void ResetBeatManager(bool resetCounter)
    {
        if (resetCounter)
        {
            globalCounterNegra = 0;
            globalCounterCorchea = 0;
        }

        lastBeatTime = 0;
        nextBeatTime = 0;
        nextHalfBeatTime = 0;
    }

    public BeatReciever.BeatFeedback EvaluateInput(BeatType type)
    {
        double duration = (type == BeatType.FullBeat)? quarterBeatDuration : eighthBeatDuration;
        double nearestTime;

        if (type == BeatType.FullBeat) nearestTime = Math.Abs(songTime - lastBeatTime)<Math.Abs(songTime - nextBeatTime)? lastBeatTime: nextBeatTime;
        else nearestTime = nextHalfBeatTime;
        
        double delta = songTime - nearestTime;
        double absDelta = Math.Abs(delta);
        double maxWindow = duration * margenPercentOnBeat;
        double greatWindow =maxWindow * greatPercentOnMargin;
        double perfectWindow = greatWindow * perfectPercentOnMargin;
        if (absDelta <= perfectWindow) return BeatReciever.BeatFeedback.Perfect;
        if (absDelta <= greatWindow) return BeatReciever.BeatFeedback.Great;
        if (absDelta <= maxWindow) return delta < 0? BeatReciever.BeatFeedback.Early : BeatReciever.BeatFeedback.Late;
        return BeatReciever.BeatFeedback.Bad;
    }

    public int GetCounter(BeatType beatType)
    {
        return (beatType==BeatType.FullBeat)? globalCounterNegra : globalCounterCorchea;
    }
}