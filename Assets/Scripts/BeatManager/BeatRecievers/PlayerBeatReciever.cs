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
            animator.SetFloat("Beat",(float)(1f/beatTime));
        }
    }

    public override void BeatAction(int counter)
    {
        base.BeatAction(counter);
        animator.SetTrigger("Pulse");
        //Invoke("ResetIdle",0.1f);
    }
    

    public override void OnPauseSongAction()
    {
        animator.enabled = false;
    }

    public void ResetIdle()
    {
        animator.ResetTrigger("Pulse");
    }
}
