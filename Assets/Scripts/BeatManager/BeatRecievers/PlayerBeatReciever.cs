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
    
    private void SetBeatDuration()
    {
        if (animator != null)
        {
            //Debug.Log("Playing Dance Animator");
            animator.SetFloat("Beat",(float)(1f/barTime));
        }
    }

    public override void PreBeatAction(int counter)
    {
        //throw new NotImplementedException();
    }

    public override void BeatAction(int counter)
    {
        animator.SetTrigger("Pulse");
        //Invoke("ResetIdle",0.1f);
    }

    public override void PostBeatAction(int counter)
    {
        //throw new NotImplementedException();
    }

    public override void OnStopSongAction()
    {
        //throw new NotImplementedException();
    }


    public override void OnPauseSongAction()
    {
        animator.enabled = false;
    }

    public override void OnResumeAction()
    {
        //throw new NotImplementedException();
    }

    public void ResetIdle()
    {
        animator.ResetTrigger("Pulse");
    }
}
