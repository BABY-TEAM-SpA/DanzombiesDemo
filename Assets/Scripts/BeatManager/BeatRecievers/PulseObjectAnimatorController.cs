using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PulseObjectAnimatorController : BeatReciever
{
    
    private double currentBeatOnPlayer = 0d;
    [SerializeField]  Animator animator ;
    [SerializeField] AnimatorOverrideController animatorOverrideController;
    
    public void Awake()
    {
        animator = GetComponent<Animator>();
        if (animatorOverrideController != null)
        { 
            animator.runtimeAnimatorController = animatorOverrideController;
            animator.enabled = true;
        }
    }

    public override void BeatAction(int counter)
    {
        animator.Play("Pulse");
        //Debug.Log(transform.name +": Pulsed at "+ AudioSettings.dspTime);
    }
    
    public override void OnPlaySongAction()
    {
        SetBeatDuration();
    }
    private void SetBeatDuration()
    {
        currentBeatOnPlayer = AudioManager.Instance.beatDuration;
        animator.enabled = true;
        animator.SetFloat("Beat",(float)(1/currentBeatOnPlayer));
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
