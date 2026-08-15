using System;
using UnityEngine;

public class PlayerBeatReciever : BeatReciever
{
    [SerializeField] Animator animator;

    public override void OnUpdateSongAction()
    {
        animator.enabled = true;
        SetBeatDuration();
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

    private void SetBeatDuration()
    {
        if (animator != null)
        {
            double duration = BeatManager.Instance? BeatManager.Instance.quarterBeatDuration:1d;
            animator.SetFloat("Beat",(float)(1f/duration));
        }
    }
    
    

    public void ResetIdle()
    {
        animator.ResetTrigger("Pulse");
    }
}
