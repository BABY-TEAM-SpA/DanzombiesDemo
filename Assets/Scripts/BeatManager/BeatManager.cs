using System;
using UnityEngine;
using UnityEngine.Events;

public enum BeatType
{
    Negra,
    Blanca,
    Redonda
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

    int lastBeat = -1;

    double dspStartTime;

    bool preTriggered;
    bool beatTriggered;
    bool postTriggered;
    
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
        if (!AudioManager.Instance.IsPlaying())
            return;

        double dspTime = AudioSettings.dspTime;

        double songTime =
            dspTime -
            AudioManager.Instance.currentSongPlaying.dspSongStartTime;

        UpdateBeat(songTime);
        //UpdateHalfBeat(songTime);
    }

    void UpdateBeat(double songTime)
    {
	    // --- TIMELINE (eventos) ---
	    int timelineBeat = (int)(songTime / beatDuration);

	    if (timelineBeat != lastBeat)
	    {
		    lastBeat = timelineBeat;
		    preTriggered = false;
		    beatTriggered = false;
		    postTriggered = false;
		    counter++;
	    }

	    double beatStart = timelineBeat * beatDuration;
	    double deltaTimeline = songTime - beatStart;

	    double maxWindow = beatDuration * margenPercentOnBeat;

	    if (!preTriggered && deltaTimeline >= -maxWindow)
	    {
		    preTriggered = true;
		    OnPreBeat?.Invoke(timelineBeat);
	    }

	    if (!beatTriggered && deltaTimeline >= 0)
	    {
		    beatTriggered = true;
		    OnBeat?.Invoke(timelineBeat);
	    }

	    if (!postTriggered && deltaTimeline >= maxWindow)
	    {
		    postTriggered = true;
		    OnPostBeat?.Invoke(timelineBeat);
	    
	    }
    }

    public BeatFeedback EvaluateInput()
    {
	    double checkTime = AudioSettings.dspTime - dspStartTime;
	    double exactBeat = checkTime / beatDuration;
	    int nearestBeat = (int)Math.Floor(exactBeat + 0.5);

	    double nearestBeatTime = nearestBeat * beatDuration;
	    double delta = checkTime - nearestBeatTime;

	    double maxWindow = beatDuration * margenPercentOnBeat;
	    double perfectWindow = beatDuration * (margenPercentOnBeat * perfectPercentOnMargin);
	    double greatWindow = beatDuration * (margenPercentOnBeat * greatPercentOnMargin);

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
        
        preTriggered = false;
        beatTriggered = false;
        postTriggered = false;


        if (resetCounter)
        {
            Debug.Log("Counter Reset");
            counter = 0;
            lastBeat = -1;
        }
    }
}