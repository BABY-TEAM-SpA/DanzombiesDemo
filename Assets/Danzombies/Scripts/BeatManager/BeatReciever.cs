using UnityEngine;

public abstract class BeatReciever : MonoBehaviour
{
    public bool isActive { get; set; } = false;
    

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
    
    private void OnBeatEvent(int beatCounter, BeatManager.BeatType beatType, int beatTimeMs)
    {
        if (!isActive) return;
        BeatAction(beatCounter, beatType, beatTimeMs);
    }
    
    public virtual void OnUpdateSongAction(double barDuration){}
    public abstract void BeatAction(int beatCounter, BeatManager.BeatType beatType, int beatTimeMs);
    

}