using System;
using System.Runtime.InteropServices;
using FMOD.Studio;
using UnityEngine;


public class BeatManager : MonoBehaviour
{
    public bool useDebug = false;

    public enum BeatType
    {
        FullBeat,
        FirThird,
        SecThird,
        HalfBeat
    }

    [Header("Sync")] [Range(0f, 0.4f)] public double margenPercentOnBeat = 0.25d;

    [Range(0f, 1f)] public double greatPercentOnMargin = 0.5d;

    [Range(0f, 0.5f)] public double perfectPercentOnMargin = 0.1d;

    public double quarterBeatDuration { get; private set; } = 1f;

    public int globalCounterNegra { get; private set; } = 1;

    public delegate void OnUpdate(double beatDuration);

    public static event OnUpdate OnUpdateEvent;
    private bool beatManagerReady;
    double songTime;

    double lastBeatTime;
    double nextBeatTime;

    double partBeatTime;
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

    void Awake()
    {
        if (Instance != null && Instance != this)
            return;
        Instance = this;
    }


    void Update()
    {
        if (!AudioManager.Instance.IsPlaying()) return;
        songTime = AudioManager.Instance.SongPositionSeconds();
        HandlePrePostBeat();
    }

    void OnPlayEvent(bool resetCounter)
    {
        ResetBeatManager(resetCounter);
        OnUpdateEvent?.Invoke(quarterBeatDuration);
    }


    public void HandleBeat(int bar, int beat, float tempo, int upper, int lower, int pos)
    {

        if (!beatManagerReady)
        {
            beatManagerReady = true;
            OnPlayEvent(false);
        }

        lastBeatTime = pos / 1000d;
        quarterBeatDuration = 60d / tempo;
        partBeatTime = quarterBeatDuration / 6;
        nextBeatTime = lastBeatTime + quarterBeatDuration;
        preTrigger = true;
        beatTrigger = true;
        postTrigger = false;
        globalCounterNegra = (beat) + ((bar - 1) * upper);
        if (useDebug) Debug.Log("beat:" + globalCounterNegra);
        OnBeat?.Invoke(globalCounterNegra, BeatType.FullBeat);

    }

    void HandlePrePostBeat()
    {
        double margin = quarterBeatDuration * margenPercentOnBeat;

        //preBeat
        if (!preTrigger && songTime >= nextBeatTime - margin)
        {
            preTrigger = true;
            beatTrigger = false;
            postTrigger = true;
            OnPreBeat?.Invoke(globalCounterNegra, BeatType.FullBeat);
        }

        //postBeat
        if (!postTrigger && beatTrigger && songTime >= lastBeatTime + margin)
        {
            preTrigger = false;
            beatTrigger = true;
            postTrigger = true;
            OnPostBeat?.Invoke(globalCounterNegra, BeatType.FullBeat);
        }
    }
    

    public void ResetBeatManager(bool resetCounter)
    {
        if (resetCounter)
        {
            globalCounterNegra = 0;
        }

        lastBeatTime = 0;
        nextBeatTime = 0;
        nextHalfBeatTime = 0;
    }

    public BeatReciever.BeatFeedback EvaluateInput()
    {
        double nearestTime;
        nearestTime = Math.Abs(songTime - lastBeatTime)<Math.Abs(songTime - nextBeatTime)? lastBeatTime: nextBeatTime;
        double delta = songTime - nearestTime;
        double absDelta = Math.Abs(delta);
        double maxWindow = partBeatTime;
        double greatWindow =maxWindow * greatPercentOnMargin;
        double perfectWindow = greatWindow * perfectPercentOnMargin;
        if (absDelta <= perfectWindow) return BeatReciever.BeatFeedback.Perfect;
        if (absDelta <= greatWindow) return BeatReciever.BeatFeedback.Great;
        if (absDelta <= maxWindow) return delta < 0? BeatReciever.BeatFeedback.Early : BeatReciever.BeatFeedback.Late;
        return BeatReciever.BeatFeedback.Bad;
    }

    public int GetCounter(BeatType beatType)
    {
        return globalCounterNegra;
    }
}