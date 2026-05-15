using System;
using UnityEngine;
using UnityEngine.Events;

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
		FullBeat,
		HalfBeat
	}
    public bool ActiveOnStart = false;

    [Header("Sincronización")]
    [Range(0f,0.4f)]
    public double margenPercentOnBeat = 0.25d;
    
    [Range(0f,1f)]
    public double greatPercentOnMargin = 0.5d;
    
    [Range(0f,0.5f)]
    public double perfectPercentOnMargin = 0.1d;

    public double beatDuration { get; private set; }
    public double beatmargen { get; private set; }

    public int counter { get; private set; }
    public BeatStatus beatStatus { get; private set; }
    
    double dspStartTime;
    private double preBeatTime;
    private double beatTime;
    private double postBeatTime;
    private double songTime;
    
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
        OnUpdateEvent?.Invoke(beatDuration);
    }

    void Update()
    {
        if (!AudioManager.Instance.IsPlaying()) return;
        songTime = AudioSettings.dspTime - AudioManager.Instance.currentSongPlaying.dspSongStartTime;
        UpdateBeat();
    }

    void UpdateBeat()
    {
	    counter = (int)Math.Round(songTime / beatDuration);
	    if (beatStatus == BeatStatus.None && songTime >= preBeatTime)
	    {
		    OnPreBeat?.Invoke(counter, BeatType.FullBeat);
		    beatStatus = BeatStatus.PreBeat;
	    }

	    if (beatStatus == BeatStatus.PreBeat && songTime >= beatTime)
	    {
		    OnBeat?.Invoke(counter, BeatType.FullBeat);
		    beatStatus = BeatStatus.PostBeat;
	    }

	    if (beatStatus == BeatStatus.PostBeat && songTime >= postBeatTime)
	    {
		    OnPostBeat?.Invoke(counter, BeatType.FullBeat);
		    beatStatus = BeatStatus.None;
		    CalculateNextBeatTime( counter+1);
	    
	    }
    }

    private void CalculateNextBeatTime(int compass)
    {
	    double margin = beatDuration * margenPercentOnBeat;
	    beatTime = compass * beatDuration;
	    preBeatTime = beatTime - margin;
	    postBeatTime = beatTime + margin;
    }
    
    public BeatReciever.BeatFeedback EvaluateInput()
    {
	    int nearestBeat = (int)Math.Round(songTime / beatDuration);
	    double nearestBeatTime = nearestBeat * beatDuration;
	    Debug.Log(nearestBeatTime);
	    double delta = songTime - nearestBeatTime;
	    Debug.Log(delta);
	    double maxWindow = beatDuration * margenPercentOnBeat;
	    double greatWindow = beatDuration * (margenPercentOnBeat * greatPercentOnMargin);
	    double perfectWindow = (beatDuration * (margenPercentOnBeat * greatPercentOnMargin))* perfectPercentOnMargin;

	    double absDelta = Math.Abs(delta);

	    if (absDelta <= perfectWindow)
	    {
		    return BeatReciever.BeatFeedback.Perfect;
	    }

	    else if (absDelta <= greatWindow)
	    {
		    return BeatReciever.BeatFeedback.Great;
	    }

	    else if (absDelta <= maxWindow)
	    {
		    BeatReciever.BeatFeedback result = delta < 0 ? BeatReciever.BeatFeedback.Early : BeatReciever.BeatFeedback.Late;
		    return result;
	    }
	    else
	    {
		    return BeatReciever.BeatFeedback.Bad;
	    }
    }

   
    public void ResetBeatManager(bool resetCounter)
    {
        beatDuration =
            AudioManager.Instance.currentSongPlaying.beatDuration;

        dspStartTime =
            AudioManager.Instance.currentSongPlaying.dspSongStartTime;

        counter = 0;
        beatStatus = BeatStatus.PreBeat;
        CalculateNextBeatTime(counter);
    }
}