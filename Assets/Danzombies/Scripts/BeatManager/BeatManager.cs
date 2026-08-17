using System;
using System.Runtime.InteropServices;
using FMOD.Studio;
using UnityEngine;


public class BeatManager : MonoBehaviour
{
    public bool useDebug = false;

    public enum BeatType
    {
        FullBeat=0,
        FirstThird=(1/3),
        HalfBeat=(1/2),
        SecondThird=(2/3)
    }

    [Header("Sync")] [Range(0f, 0.4f)] public double margenPercentOnBeat = 0.2d;

    [Range(0f, 1f)] public double greatPercentOnMargin = 0.5d;

    [Range(0f, 0.5f)] public double perfectPercentOnMargin = 0.1d;

    public double BeatTimeSec { get; private set; } = 1f;
    public double HalfBeatTimeSec { get; private set; } = 1f;
    public double ThirdBeatTimeSec { get; private set; } = 1f;

    public int localBeatCount { get; private set; } = 1; 
    public int globalBeatCount { get; private set; } = 1;
    public int globalBarCount { get; private set; } = 1;
    public int globalUpperBar { get; private set; } = 1;
    public int globalLowerBar { get; private set; } = 1;

    public delegate void OnUpdate(double beatDuration);

    public static event OnUpdate OnUpdateEvent;
    private bool isBeating;
    double songTime;

    double lastBeatTime;
    double nextBeatTime;

    double nextHalfBeatTime;

    private bool preTrigger;
    private bool beatTrigger;
    private bool postTrigger;
    

    public delegate void OnBeatEvent(int counter, BeatType beatType);

    public static event OnBeatEvent OnPreBeat;
    public static event OnBeatEvent OnBeat;
    public static event OnBeatEvent OnPostBeat;
    
    public static event OnBeatEvent OnFirsThirdPreBeat;
    public static event OnBeatEvent OnFirsThirdBeat;
    public static event OnBeatEvent OnFirsThirdPostBeat;
    
    public static event OnBeatEvent OnHalfPreBeat;
    public static event OnBeatEvent OnHalfBeat;
    public static event OnBeatEvent OnHalfPostBeat;
    
    public static event OnBeatEvent OnSecondThirdPreBeat;
    public static event OnBeatEvent OnSecondThirdBeat;
    public static event OnBeatEvent OnSecondThirdPostBeat;

    public static BeatManager Instance { get; private set; }

    EventInstance trackedMusic;

    void Awake()
    {
        if (Instance != null && Instance != this)
            return;
        Instance = this;
    }
    
    void OnPlayEvent(float tempo)
    {
        BeatTimeSec = 60d / tempo;
        HalfBeatTimeSec = BeatTimeSec / 2d;
        ThirdBeatTimeSec = BeatTimeSec / 3d;
        isBeating=true;
        OnUpdateEvent?.Invoke(BeatTimeSec);
    }
    public void HandleBeat(int bar, int beat, float tempo, int upper, int lower, int pos)
    {
        if (beat==1 && bar==1 && !isBeating) OnPlayEvent(tempo);
        preTrigger = true;
        beatTrigger = true;
        postTrigger = false;
        
        localBeatCount = beat;
        globalBeatCount = (beat) + ((bar - 1) * upper);
        globalBarCount = bar;
        globalUpperBar = upper;
        globalLowerBar = lower;
        
        if (useDebug) Debug.Log("beat:" + globalBeatCount);
        OnBeat?.Invoke(globalBeatCount, BeatType.FullBeat); ///1, 2 ,3, 4, 1, 2, 3, 4 (segun el Upper)
    }
    void Update()
    {
        if (!AudioManager.Instance.IsPlaying()) return;
        songTime = AudioManager.Instance.SongPositionSeconds();
        HandlePrePostBeat();
    }

    void HandlePrePostBeat()
    {
        double margin = BeatTimeSec * margenPercentOnBeat;

        //preBeat
        if (!preTrigger && songTime >= nextBeatTime - margin)
        {
            preTrigger = true;
            beatTrigger = false;
            postTrigger = true;
            OnPreBeat?.Invoke(localBeatCount, BeatType.FullBeat);
        }

        //postBeat
        if (!postTrigger && beatTrigger && songTime >= lastBeatTime + margin)
        {
            preTrigger = false;
            beatTrigger = true;
            postTrigger = true;
            OnPostBeat?.Invoke(localBeatCount, BeatType.FullBeat);
        }
    }
    

    public BeatReciever.BeatFeedback EvaluateInput(int inputBeat, BeatType inputBeatType)
    {
        //Debug.Log(inputBeat);
        double inputTime = AudioManager.Instance.SongPositionSeconds();;
        //Debug.Log(inputTime);
        double BeatTime = BeatTimeSec*(inputBeat-1) + BeatTimeSec*(float)inputBeatType;
        //Debug.Log(BeatTime);
        double delta = BeatTime - inputTime;
        //Debug.Log(delta);
        double absDelta = Math.Abs(delta);
        double maxWindow = BeatTimeSec*margenPercentOnBeat;
        //Debug.Log(maxWindow);
        double greatWindow =maxWindow * greatPercentOnMargin;
        double perfectWindow = greatWindow * perfectPercentOnMargin;
        if (absDelta <= perfectWindow) return BeatReciever.BeatFeedback.Perfect;
        if (absDelta <= greatWindow) return BeatReciever.BeatFeedback.Great;
        if (absDelta <= maxWindow) return delta < 0? BeatReciever.BeatFeedback.Early : BeatReciever.BeatFeedback.Late;
        return BeatReciever.BeatFeedback.Bad;
    }
}