using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
public enum DanceLean
{
    None,
    L,
    R,
}

public enum DanceDirection{
    None,
    North,
    South,
    West,
    East
}

[Serializable]
public class AnimationFeedback
{
    public string name;
    public UnityEvent feedbackEvent;
}
public class DanceAnimatorController : MonoBehaviour
{
    [SerializeField] protected DanceBrain _danceBrain;
    [SerializeField] public Animator animator;
    private AnimatorOverrideController alphaOverrider;
    private AnimatorOverrideController betaOverrider;
    private double currentBeatOnPlayer = 0d;
    private bool isDirectionPulsed;
    private DanceDirection isDancePulsed;

    public List<AnimationFeedback> playerFeedbackEvents = new List<AnimationFeedback>();
    
    private void Start()
    {
        SetAnimatorOverrideDirection();
    }
    
    public void AnimateOnMoving(Vector3 velocity)
    {
        bool moving = velocity != Vector3.zero;
        animator?.SetBool("LeftLooking", _danceBrain.isLeftLooking);
        animator?.SetBool("Walking", moving);
        animator?.SetFloat("WalkingSpeed", velocity.magnitude);
    }
    
    public void OnDanceBegin(DanceStep step)
    {
        if (step == DanceStep.None) return; 
        _danceBrain?.EnableMovement(false);
        animator.Play(step.ToString(), 0,0f);
    }

    public void OnStandAction()
    {
        _danceBrain.EnableMovement(true);
    }

    public void SetExpression(AnimatorOverrideController alpha, AnimatorOverrideController beta)
    {
        alphaOverrider = alpha;
        betaOverrider = beta;
        SetAnimatorOverrideDirection();
    }
    
    public void SetAnimatorOverrideDirection()
    {
        bool isLeft = _danceBrain.isLeftLooking;
        animator.SetBool("LeftLooking", isLeft);
        animator.runtimeAnimatorController = isLeft? alphaOverrider : betaOverrider;
    }
    

    public void AnimationFeedbackEvent(string eventName)
    {
        playerFeedbackEvents.FirstOrDefault(x=> x.name==eventName)?.feedbackEvent?.Invoke();
    }
}
