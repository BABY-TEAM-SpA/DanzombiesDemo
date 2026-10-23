using System;
using UnityEngine;

public class PlayerBeatReciever : BeatReciever
{
    [SerializeField] DanceBrain brain;

    public void Start()
    {
        if (brain == null) brain = GetComponent<DanceBrain>();
    }

    public override void OnUpdateSongAction(double barDuration)
    {
        brain?.UpdateAnimator(barDuration);
    }
    
    public override void BeatAction(int beat, BeatManager.BeatType type, int beatTimeMs)
    {
        if (brain.isBussy) return;
        //if(type == BeatManager.BeatType.FullBeat) brain.OnDanceStepAction();
    }
}
