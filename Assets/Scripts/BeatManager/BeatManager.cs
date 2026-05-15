using System;
using UnityEngine;

public enum BeatStatus
{
    PreBeat,
    PostBeat,
    None
}

public class BeatManager : MonoBehaviour
{
    public enum BeatType
    {
        FullBeat,   // Negras
        HalfBeat    // Corcheas
    }

    public bool ActiveOnStart = false;

    [Header("Sincronización")]
    [Range(0f, 0.4f)]
    public double margenPercentOnBeat = 0.25d;

    [Range(0f, 1f)]
    public double greatPercentOnMargin = 0.5d;

    [Range(0f, 0.5f)]
    public double perfectPercentOnMargin = 0.1d;

    public double quarterBeatDuration { get; private set; }
    public double eighthBeatDuration { get; private set; }

    public int quarterCounter { get; private set; }
    public int eighthCounter { get; private set; }

    private BeatStatus quarterStatus = BeatStatus.None;
    private BeatStatus eighthStatus = BeatStatus.None;

    private double dspStartTime;
    private double songTime;

    private double preBeatTime;
    private double beatTime;
    private double postBeatTime;

    private double preHalfBeatTime;
    private double halfBeatTime;
    private double postHalfBeatTime;
    // Eventos
    public delegate void OnUpdate(double beatDuration);
    public static event OnUpdate OnUpdateEvent;

    public delegate void OnBeatEvent(int counter, BeatType beatType);
    public static event OnBeatEvent OnPreBeat;
    public static event OnBeatEvent OnBeat;
    public static event OnBeatEvent OnPostBeat;

    public static BeatManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnEnable()
    {
        AudioManager.OnPlay += OnPlayEvent;
    }

    void OnDisable()
    {
        AudioManager.OnPlay -= OnPlayEvent;
    }

    void OnPlayEvent(bool resetCounter)
    {
        ResetBeatManager(resetCounter);
        OnUpdateEvent?.Invoke(quarterBeatDuration);
    }

    void Update()
    {
        if (!AudioManager.Instance.IsPlaying()) return;

        songTime = AudioSettings.dspTime -
                   AudioManager.Instance.currentSongPlaying.dspSongStartTime;

        UpdateNegrasBeat();
        UpdateCorcheasBeat();
    }

    // =========================
    // FULL BEAT

    void UpdateNegrasBeat()
    {
        quarterCounter = (int)Math.Round(songTime / quarterBeatDuration);

        if (quarterStatus == BeatStatus.None && songTime >= preBeatTime)
        {
            OnPreBeat?.Invoke(quarterCounter, BeatType.FullBeat);
            quarterStatus = BeatStatus.PreBeat;
        }

        if (quarterStatus == BeatStatus.PreBeat && songTime >= beatTime)
        {
            OnBeat?.Invoke(quarterCounter, BeatType.FullBeat);
            Debug.Log("FullBeat");
            quarterStatus = BeatStatus.PostBeat;
        }

        if (quarterStatus == BeatStatus.PostBeat && songTime >= postBeatTime)
        {
            OnPostBeat?.Invoke(quarterCounter, BeatType.FullBeat);
            quarterStatus = BeatStatus.None;
            CalculateNextBeatTime( quarterCounter+1);
        }
    }

    // =========================
    // HALF BEAT 

    void UpdateCorcheasBeat()
    {
        eighthCounter = (int)Math.Round(songTime / eighthBeatDuration);
        if (eighthStatus == BeatStatus.None && songTime >= preHalfBeatTime)
        {
            OnPreBeat?.Invoke(eighthCounter, BeatType.HalfBeat);
            eighthStatus = BeatStatus.PreBeat;
        }

        if (eighthStatus == BeatStatus.PreBeat && songTime >= halfBeatTime)
        {
            OnBeat?.Invoke(eighthCounter, BeatType.HalfBeat);
            Debug.Log("HalfBeat");
            eighthStatus = BeatStatus.PostBeat;
        }

        if (eighthStatus == BeatStatus.PostBeat && songTime >= postHalfBeatTime)
        {
            OnPostBeat?.Invoke(eighthCounter, BeatType.HalfBeat);
            eighthStatus = BeatStatus.None;
            CalculateNextHalfBeatTime(eighthCounter+1);
        }
    }


    public BeatReciever.BeatFeedback EvaluateInput(BeatType type)
    {
        double duration = type == BeatType.FullBeat
            ? quarterBeatDuration
            : eighthBeatDuration;

        int nearestBeat = (int)Math.Round(songTime / duration);
        double nearestBeatTime = nearestBeat * duration;

        double delta = songTime - nearestBeatTime;
        double absDelta = Math.Abs(delta);

        double maxWindow = duration * margenPercentOnBeat;
        double greatWindow = maxWindow * greatPercentOnMargin;
        double perfectWindow = greatWindow * perfectPercentOnMargin;

        if (absDelta <= perfectWindow)
            return BeatReciever.BeatFeedback.Perfect;

        if (absDelta <= greatWindow)
            return BeatReciever.BeatFeedback.Great;

        if (absDelta <= maxWindow)
            return delta < 0
                ? BeatReciever.BeatFeedback.Early
                : BeatReciever.BeatFeedback.Late;

        return BeatReciever.BeatFeedback.Bad;
    }


    public void ResetBeatManager(bool resetCounter)
    {
        quarterBeatDuration =
            AudioManager.Instance.currentSongPlaying.beatDuration;

        eighthBeatDuration = quarterBeatDuration * 0.5d;

        dspStartTime =
            AudioManager.Instance.currentSongPlaying.dspSongStartTime;

        quarterCounter = 0;
        eighthCounter = 0;

        quarterStatus = BeatStatus.None;
        eighthStatus = BeatStatus.None;
    }

    public int GetCounter(BeatType beatType)
    {
        switch (beatType)
        {
            case BeatType.FullBeat:
                return quarterCounter;
            case BeatType.HalfBeat:
                return eighthCounter;
            default:
                return 0;
        }
    }

    private void CalculateNextBeatTime(int compass)
    {
        double margin = quarterBeatDuration * margenPercentOnBeat;
        beatTime = compass * quarterBeatDuration;
        preBeatTime = beatTime - margin;
        postBeatTime = beatTime + margin;
    }
    private void CalculateNextHalfBeatTime(int compass)
    {
        double margin = eighthBeatDuration * margenPercentOnBeat;
        halfBeatTime = compass * eighthBeatDuration;
        preHalfBeatTime = halfBeatTime - margin;
        postHalfBeatTime = halfBeatTime + margin;
            
    }
}