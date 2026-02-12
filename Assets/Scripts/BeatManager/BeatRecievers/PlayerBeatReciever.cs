using System;
using UnityEngine;

public class PlayerBeatReciever : BeatReciever
{
    [SerializeField] Animator animator;
    private double currentBeatOnPlayer = 0d;

    public override void OnPlaySongAction()//double beatDuration)
    {
        animator.enabled = true;
        SetBeatDuration();
    }
    
    private void SetBeatDuration()
    {
        currentBeatOnPlayer = AudioManager.Instance.beatDuration;
        if (animator != null)
        {
            //Debug.Log("Playing Dance Animator");
            animator.SetFloat("Beat",(float)(1/currentBeatOnPlayer));
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
