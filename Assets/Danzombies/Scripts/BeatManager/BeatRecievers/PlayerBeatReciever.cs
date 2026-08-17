using System;
using UnityEngine;

public class PlayerBeatReciever : BeatReciever
{
    [SerializeField] Animator animator;

    public override void OnUpdateSongAction(double barDuration)
    {
        animator.enabled = true;
        double duration = barDuration;
        float value = (float)(1f / duration);
        animator.SetFloat("Beat",value);
    }

    public override void PreBeatAction(int beat, BeatManager.BeatType type)
    {
        //throw new NotImplementedException();
    }

    public override void BeatAction(int beat, BeatManager.BeatType type)
    {
        if(type == BeatManager.BeatType.FullBeat) animator.SetTrigger("Pulse");
    }

    public override void PostBeatAction(int beat, BeatManager.BeatType type)
    {
        //throw new NotImplementedException();
    }
    

    public void ResetIdle()
    {
        animator.ResetTrigger("Pulse");
    }
}
