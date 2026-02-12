using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PulseObjectAnimatorController : BeatReciever
{
    
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
    
    public override void OnUpdateSongAction()
    {
        animator.enabled=true;
        SetBeatDuration();
    }
    private void SetBeatDuration()
    {
        animator.SetFloat("Beat",(float)(1f/this.beatTime));
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
