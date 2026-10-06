using System;
using System.Collections.Generic;
using UnityEngine;


public class BeatManager : Service<BeatManager>
{
    #region [VARIABLES]
    public bool useDebug = false;
    private bool isBeating;

    public enum BeatType { FullBeat=0, FirstThird=4, SecondThird=8,HalfBeat=6 }
    
    [Header("Sync")] 
    [Range(0f, 0.4f)] public double margenPercentOnBeat = 0.08d;

    [Range(0f, 1f)] public double greatPercentOnMargin = 0.5d;

    [Range(0f, 0.5f)] public double perfectPercentOnMargin = 0.1d;

    public double BeatTimeSec { get; private set; } = 1d;
    
    // Bar duration
    public double lastBeatTime { get; private set; } = 0d;
    public double nextBeatTime => lastBeatTime + BeatTimeSec;
    public double margin => BeatTimeSec * margenPercentOnBeat;
    
    public int localBeatCount { get; private set; } = 1; 
    public int globalBeatCount { get; private set; } = 1;
    public int globalBarCount { get; private set; } = 1;
    public int globalUpperBar { get; private set; } = 1;
    public int globalLowerBar { get; private set; } = 1;
    

    public delegate void OnUpdate(double beatDuration);

    public static event OnUpdate OnUpdateEvent;
    
    double songTime;
    private BeatChannel.BeatChannelState mainBeatState = BeatChannel.BeatChannelState.Waiting;
    
    public delegate void OnBeatEvent(int counter, BeatType beatType);

    public static event OnBeatEvent OnPreBeat;
    public static event OnBeatEvent OnBeat;
    public static event OnBeatEvent OnPostBeat;
    
    //EventInstance trackedMusic;
    #endregion
    
    
    #region BeatBuffer
    
    [SerializeField] List<BeatChannel> beatChannels = new List<BeatChannel>();
    
    [Serializable]
public class BeatChannel 
{
    private double channelLastBeatTime;
    private double channelNextBeatTime;
    private bool alreadyBeated;

    public enum BeatChannelState { Waiting, Pre, Post }
    public BeatChannelState channelState;
    public BeatManager.BeatType channelBeatType;

    public void SetNewTime() 
    {
        channelNextBeatTime = Instance.lastBeatTime + (Instance.BeatTimeSec * (double)channelBeatType / 12d);
        channelState = BeatChannelState.Waiting;
        alreadyBeated = false; 
    }

    public void Update(double songTime) 
    {
        if (alreadyBeated) return; 
        switch (channelState) 
        {
            case BeatChannelState.Waiting:
                if (songTime >= channelNextBeatTime - Instance.margin) 
                {
                    OnPreBeat?.Invoke(Instance.globalBeatCount, channelBeatType);
                    if(Instance.useDebug) Debug.Log($"--{channelBeatType}--PreBeat: {Instance.globalBeatCount}");
                    channelState = BeatChannelState.Pre;
                }
                break;

            case BeatChannelState.Pre:
                if (songTime >= channelNextBeatTime) 
                {
                    channelLastBeatTime = channelNextBeatTime;
                    OnBeat?.Invoke(Instance.globalBeatCount, channelBeatType);
                    if(Instance.useDebug) Debug.Log($"--{channelBeatType}-*-Beat: {Instance.globalBeatCount}");
                    channelState = BeatChannelState.Post;
                }
                break;

            case BeatChannelState.Post:
                if (songTime >= channelLastBeatTime + Instance.margin) 
                {
                    OnPostBeat?.Invoke(Instance.globalBeatCount, channelBeatType);
                    if(Instance.useDebug) Debug.Log($"--{channelBeatType}--Postbeat: {Instance.globalBeatCount}");
                    alreadyBeated = true; 
                }
                break;
        }
    }
}

    #endregion
    
    
    #region [METHODS]
    private void OnEnable()
    {
        AudioManager.OnPlay += OnSongPlay;
        AudioManager.OnPause += OnSongPaused;
        AudioManager.OnStop += OnSongStopped;
    }

    private void OnDisable()
    {
        AudioManager.OnPlay -= OnSongPlay;
        AudioManager.OnPause -= OnSongPaused;
        AudioManager.OnStop -= OnSongStopped;
    }

    void OnSongPlay()
    {
        isBeating=true;
    }
    public void OnSongPaused()=> isBeating=false;
    private void OnSongStopped() => isBeating = false;
    
    public void HandleBeat(int bar, int beat, float tempo, int upper, int lower, int pos)
    {
        lastBeatTime = AudioManager.Instance.SongPositionSeconds();
        BeatTimeSec = 60d / tempo;
        OnUpdateEvent?.Invoke(BeatTimeSec);
        localBeatCount = beat;
        globalBeatCount = (beat) + ((bar - 1) * upper);
        globalBarCount = bar;
        globalUpperBar = upper;
        globalLowerBar = lower;
        if(useDebug) Debug.Log($"-*-{BeatType.FullBeat}:Beat: {localBeatCount}");
        OnBeat?.Invoke(globalBeatCount, BeatType.FullBeat); //1, 2 ,3, 4, 1, 2, 3, 4 (segun el Upper)
        mainBeatState = BeatChannel.BeatChannelState.Post;
        beatChannels[0]?.SetNewTime();
        beatChannels[1]?.SetNewTime();
        beatChannels[2]?.SetNewTime();
 
    }
    
    void Update()
    {
        if (!AudioManager.Instance.IsPlaying()) return;
        songTime = AudioManager.Instance.SongPositionSeconds();
        UpdateMainBeat();
        //UpdateBuffers();
    }

    private void UpdateMainBeat()
    {
        switch (mainBeatState)
        {
            case BeatChannel.BeatChannelState.Waiting:
                if (songTime >= nextBeatTime - margin)
                {
                    localBeatCount = (localBeatCount+1<=globalUpperBar)?localBeatCount+1:1;
                    OnPreBeat?.Invoke(localBeatCount,BeatType.FullBeat);
                    //OnPreBeat?.Invoke(globalBeatCount,BeatType.FullBeat);
                    if(useDebug) Debug.Log($"-{BeatType.FullBeat}:PreBeat: {localBeatCount}");
                    mainBeatState = BeatChannel.BeatChannelState.Pre;
                }
                break;
            case BeatChannel.BeatChannelState.Post:
                if(songTime>= lastBeatTime + margin)
                {
                    OnPostBeat?.Invoke(localBeatCount, BeatType.FullBeat);
                    //OnPostBeat?.Invoke(globalBeatCount, BeatType.FullBeat);
                    if(useDebug) Debug.Log($"-{BeatType.FullBeat}:PostBeat: {localBeatCount}");
                    mainBeatState = BeatChannel.BeatChannelState.Waiting;
                }
                break;
        }
    }

    private void UpdateBuffers()
    {
        //beatChannels[0]?.Update(songTime);
        beatChannels[1]?.Update(songTime);
        //beatChannels[2]?.Update(songTime);
    }
    
    
    public BeatReciever.BeatFeedback EvaluateInput(int inputBeat, BeatType inputBeatType, int StepParts=1)
    {
        //Debug.Log(inputBeat);
        double inputTime = AudioManager.Instance.SongPositionSeconds();;
        //Debug.Log(inputTime);
        double BeatTime = BeatTimeSec*(inputBeat-1) + BeatTimeSec*(float)inputBeatType;
        //Debug.Log(BeatTime);
        double delta = BeatTime - inputTime;
        //Debug.Log(delta);
        double absDelta = Math.Abs(delta);
        double maxWindow = BeatTimeSec*margenPercentOnBeat*3/StepParts;
        //Debug.Log(maxWindow);
        double greatWindow =maxWindow * greatPercentOnMargin;
        double perfectWindow = greatWindow * perfectPercentOnMargin;
        if (absDelta <= perfectWindow) return BeatReciever.BeatFeedback.Perfect;
        if (absDelta <= greatWindow) return BeatReciever.BeatFeedback.Good;
        if (absDelta <= maxWindow) return delta < 0? BeatReciever.BeatFeedback.Early : BeatReciever.BeatFeedback.Late;
        return BeatReciever.BeatFeedback.Bad;
    }
    #endregion
}