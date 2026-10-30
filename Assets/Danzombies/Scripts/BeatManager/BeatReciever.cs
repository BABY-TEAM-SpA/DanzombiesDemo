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
        if(beatType == BeatManager.BeatType.HalfBeat) PostBarAction(beatCounter);
        
    }
    
    public virtual void OnUpdateSongAction(double barDuration){}
    /** beatCounter(int) starts counting the beats 1,2,3,4 to upper (it loops),
     * beatType(int enum) is the part of the beat [0,1,2,3,4,5],
     * beatTimeMs(int) is the songTime [Miliseconds: 0000ms] where the beatPart is launched (it loops)*
     */
    public abstract void BeatAction(int beatCounter, BeatManager.BeatType beatType, int beatTimeMs);

    public virtual void PostBarAction(int beatCounter) { }


}