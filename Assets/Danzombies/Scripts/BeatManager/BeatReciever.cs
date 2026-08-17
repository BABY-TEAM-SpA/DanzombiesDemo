using UnityEngine;

public abstract class BeatReciever : MonoBehaviour
{
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
        OnUpdateSongAction( barDuration);
    }

    private void OnPreBeatEvent(int beat, BeatManager.BeatType type)
    {
        if (!isActive) return;
        PreBeatAction(beat, type);
        isOnBeat = true;
    }

    private void OnBeatEvent(int beat, BeatManager.BeatType type)
    {
        if (!isActive) return;
        BeatAction(beat, type);
    }

    private void OnPostBeatEvent(int beat, BeatManager.BeatType type)
    {
        if (!isActive) return;
        PostBeatAction(beat, type);
        isOnBeat = false;
    }

    public virtual void OnUpdateSongAction(double barDuration){}
    public abstract void PreBeatAction(int beat, BeatManager.BeatType type);
    public abstract void BeatAction(int beat, BeatManager.BeatType type);
    public abstract void PostBeatAction(int beat, BeatManager.BeatType type);
    

}