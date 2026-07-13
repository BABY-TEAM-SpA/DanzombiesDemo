using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PulseObjectAnimatorController : BeatReciever
{
    
    [SerializeField] Animator animator ;
    [SerializeField] AnimatorOverrideController animatorOverrideController;
    
    public void Awake()
    {
        animator = GetComponent<Animator>();
        if (animatorOverrideController != null)
        { 
            animator.runtimeAnimatorController = animatorOverrideController;
            animator.enabled = true;
            isActive=true;
        }
    }

    public override void PreBeatAction(int counter)
    {
        //throw new System.NotImplementedException();
    }

    public override void BeatAction(int counter)
    {
        animator.SetTrigger("Pulse");
        Debug.Log(counter);
    }

    public override void PostBeatAction(int counter)
    {
        animator.ResetTrigger("Pulse");
    }

    public override void OnUpdateSongAction()
    {
        animator.enabled=true;
        SetBeatDuration();
    }
    private void SetBeatDuration()
    {
        double duration = BeatManager.Instance? BeatManager.Instance.eighthBeatDuration:1d;
        animator.SetFloat("Beat",(float)(1f/duration));
    }

   
}
