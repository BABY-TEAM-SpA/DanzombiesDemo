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
    
    public override void BeatAction(int beat, BeatManager.BeatType type, int beatTimeMs)
    {
        if(type == BeatManager.BeatType.FullBeat) animator.SetTrigger("Pulse");
    }

    public void ResetIdle()
    {
        animator.ResetTrigger("Pulse");
    }
}
