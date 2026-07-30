using UnityEngine;
using static BeatManager;

public abstract class BeatReciever : MonoBehaviour
{
    protected BeatType beatType = BeatType.FullBeat;
    public bool isActive { get; set; } = false;
    public bool isOnBeat { get; private set; } = false;
    

    public enum BeatFeedback
    {
        Bad,
        Early,
        Great,
        Perfect,
        Late
    }

    private void OnEnable()
    {
        BeatManager.OnUpdateEvent += OnUpdateSongEvent;
        BeatManager.OnPreBeat += OnPreBeatEvent;
        BeatManager.OnBeat += OnBeatEvent;
        BeatManager.OnPostBeat += OnPostBeatEvent;
    }

    private void OnDisable()
    {
        BeatManager.OnUpdateEvent -= OnUpdateSongEvent;
        BeatManager.OnPreBeat -= OnPreBeatEvent;
        BeatManager.OnBeat -= OnBeatEvent;
        BeatManager.OnPostBeat -= OnPostBeatEvent;
    }
    public void SetActive(bool active)
    {
        isActive = active;
    }

    private void OnUpdateSongEvent(double barDuration)
    {
        OnUpdateSongAction();
    }

    private void OnPreBeatEvent(int beat, BeatType type)
    {
        if (!isActive) return;
        if (type != beatType) return;
        PreBeatAction(beat);
        isOnBeat = true;
    }

    private void OnBeatEvent(int beat, BeatType type)
    {
        if (!isActive) return;
        if (type != beatType) return;
        BeatAction(beat);
    }

    private void OnPostBeatEvent(int beat, BeatType type)
    {
        if (!isActive) return;
        if (type != beatType) return; 
        PostBeatAction(beat);
        isOnBeat = false;
    }

    public virtual void OnUpdateSongAction(){}
    public abstract void PreBeatAction(int counter);
    public abstract void BeatAction(int counter);
    public abstract void PostBeatAction(int counter);
    
}