using System;
using System.Collections.Generic;
using UnityEngine;


public class BeatManager : Service<BeatManager>
{
    #region [VARIABLES]
    public bool useDebug = false;
    private bool isBeating;

    
    
    [Header("Sync")] 
    [Range(0f, 0.4f)] public float margenPercentOnBeat = 0.08f;
    [Range(0f, 1f)] public float greatPercentOnMargin = 0.5f;
    [Range(0f, 0.5f)] public float perfectPercentOnMargin = 0.1f;

    public int BeatDurationMs { get; private set; } = 1;
    private float beatPartTimeMs;
    
    // Bar duration
    public int lastBeatTime { get; private set; } = 0;
    public float nextBeatTime;
    public float margin => BeatDurationMs * margenPercentOnBeat;
    private bool shouldBeatPart;
    
    public int localBeatCount { get; private set; } = 1; 
    //public int globalBeatCount { get; private set; } = 1;
    public int globalBarCount { get; private set; } = 1;
    public int globalUpperBar { get; private set; } = 1;
    public int globalLowerBar { get; private set; } = 1;
    

    public delegate void OnUpdate(double beatDuration);
    public static event OnUpdate OnUpdateEvent;
    
    double songTime;
    public enum BeatType { FullBeat=0, FirstEmpty=1, FirstThird=2, HalfBeat=3, SecondThird=4, SecondEmpty=5 }
    public BeatType mainBeatState { get; private set; }= BeatType.FullBeat;
    
    public delegate void OnBeatEvent(int counter, BeatType beatType, int beatTimeMs);
    //public static event OnBeatEvent OnPreBeat;
    public static event OnBeatEvent OnBeat;
    //public static event OnBeatEvent OnPostBeat;
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
    
    public void HandleBeat(int bar, int beat, float tempo, int upper, int lower, int posMs)
    {
        lastBeatTime = posMs;
        BeatDurationMs = Mathf.RoundToInt(60000 / tempo);
        beatPartTimeMs = BeatDurationMs / 6;
        OnUpdateEvent?.Invoke(BeatDurationMs/1000d); // tiempo a segundos
        localBeatCount = beat;
        //globalBeatCount = (beat) + ((bar - 1) * upper);
        globalBarCount = bar;
        globalUpperBar = upper;
        globalLowerBar = lower;
        
        mainBeatState = BeatType.FullBeat;
        if(useDebug) Debug.Log($"Beat: {localBeatCount}-*-{mainBeatState}");
        OnBeat?.Invoke(localBeatCount, BeatType.FullBeat, posMs); //1, 2 ,3, 4, 1, 2, 3, 4 (segun el Upper)
        SetNextBeatPartTime();
    }
    
    void Update()
    {
        if (!AudioManager.Instance.IsPlaying()) return;
        songTime = AudioManager.Instance.SongPositionMs();
        if (songTime >= nextBeatTime && shouldBeatPart) UpdatePartBeat();
    }

    private void SetNextBeatPartTime()
    {
        nextBeatTime = lastBeatTime + beatPartTimeMs*((int)mainBeatState+1);
        mainBeatState = (BeatType)((int)(mainBeatState+1)%Enum.GetNames(typeof(BeatType)).Length);
        shouldBeatPart=(mainBeatState != BeatType.FullBeat);
    }

    private void UpdatePartBeat()
    {
        
        if(useDebug) Debug.Log($"Beat: {localBeatCount}-{mainBeatState}");
        OnBeat?.Invoke(localBeatCount, mainBeatState, Mathf.RoundToInt(nextBeatTime));
        SetNextBeatPartTime();
    }
    #endregion
}