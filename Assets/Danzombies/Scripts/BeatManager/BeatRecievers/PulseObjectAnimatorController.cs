using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PulseObjectAnimatorController : BeatReciever
{
    
    [SerializeField] Animator animator ;
    [SerializeField] AnimatorOverrideController animatorOverrideController;
    [SerializeField]  BeatManager.BeatType beatMode = BeatManager.BeatType.FullBeat;
    
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

    public override void PreBeatAction(int beat, BeatManager.BeatType type)
    {
        //throw new System.NotImplementedException();
    }

    public override void BeatAction(int beat, BeatManager.BeatType type)
    {
        if(type == BeatManager.BeatType.FullBeat) animator.SetTrigger("Pulse");
        //Debug.Log(counter);
    }

    public override void PostBeatAction(int beat, BeatManager.BeatType type)
    {
        animator.ResetTrigger("Pulse");
    }

    public override void OnUpdateSongAction(double barDuration)
    {
        animator.enabled=true;
        double duration = barDuration;
        float value = (float)(1f / duration);
        animator.SetFloat("Beat",value);
    }
    

   
}
