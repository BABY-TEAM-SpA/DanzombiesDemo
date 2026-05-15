using System;
using UnityEngine;
using UnityEngine.Events;

public enum BeatStatus
{
    PreBeat,
    PostBeat,
    None
}

public enum BeatFeedback
{
    Bad,
    Early,
	Great,
    Perfect,
	Late
}

public class BeatManager : MonoBehaviour
{
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

    public delegate void OnBeatEvent(int counter);
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
		    OnPreBeat?.Invoke(counter);
		    beatStatus = BeatStatus.PreBeat;
	    }

	    if (beatStatus == BeatStatus.PreBeat && songTime >= beatTime)
	    {
		    OnBeat?.Invoke(counter);
		    beatStatus = BeatStatus.PostBeat;
	    }

	    if (beatStatus == BeatStatus.PostBeat && songTime >= postBeatTime)
	    {
		    OnPostBeat?.Invoke(counter);
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
    
    public BeatFeedback EvaluateInput()
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
		    return BeatFeedback.Perfect;
	    }

	    else if (absDelta <= greatWindow)
	    {
		    return BeatFeedback.Great;
	    }

	    else if (absDelta <= maxWindow)
	    {
		    BeatFeedback result = delta < 0 ? BeatFeedback.Early : BeatFeedback.Late;
		    return result;
	    }
	    else
	    {
		    return BeatFeedback.Bad;
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