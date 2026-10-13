using UnityEngine;

public abstract class BeatReciever : MonoBehaviour
{
    public bool isActive { get; set; } = false;
    public bool isOnBeat { get; private set; } = false;
    

    public enum BeatFeedback
    {
        Bad,
        Ok,  
        Good,   
        Perfect,
        Ignored
    }

    private void OnEnable()
    {
        BeatManager.OnUpdateEvent += OnUpdateSongEvent;
        BeatManager.OnBeat += OnBeatEvent;
    }

    private void OnDisable()
    {
        BeatManager.OnUpdateEvent -= OnUpdateSongEvent;
        BeatManager.OnBeat -= OnBeatEvent;
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
        //PreBeatAction(beat, type);
        isOnBeat = true;
    }

    private void OnBeatEvent(int beatCounter, BeatManager.BeatType beatType, int beatTimeMs)
    {
        if (!isActive) return;
        BeatAction(beatCounter, beatType, beatTimeMs);
    }

    private void OnPostBeatEvent(int beat, BeatManager.BeatType type)
    {
        if (!isActive) return;
        //PostBeatAction(beat, type);
        isOnBeat = false;
    }

    public virtual void OnUpdateSongAction(double barDuration){}
    //public abstract void PreBeatAction(int beat, BeatManager.BeatType type);
    public abstract void BeatAction(int beatCounter, BeatManager.BeatType beatType, int beatTimeMs);
    //public abstract void PostBeatAction(int beat, BeatManager.BeatType type);
    

}