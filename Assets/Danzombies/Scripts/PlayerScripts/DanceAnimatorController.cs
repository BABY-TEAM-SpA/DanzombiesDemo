using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum DanceDirection{
    None,
    North,
    South,
    West,
    East
}

public enum DanceLean
{
    None,
    L,
    R,
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
    [SerializeField] protected bool allowInput = false;
    [SerializeField] public Animator animator;
    //[SerializeField] private SpriteRenderer renderer;
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
    
    
    public void OnMoving(Vector3 velocity)
    {
        bool moving = velocity != Vector3.zero;
        animator.SetBool("LeftLooking", _danceBrain.isLeftLooking);
        if (moving)
        {
            animator.ResetTrigger("Pulse");
        }
        animator.SetBool("Walking", moving);
        animator.SetFloat("WalkingSpeed", velocity.magnitude);
    }
    
    public void OnDanceBegin(DanceStep step)
    {
        _danceBrain?.EnableMovement(false);
        _danceBrain.OnDance(step);
        animator.Play(step.ToString(), 0,0f);
    }
    public void OnStandAction()
    {
        if(_danceBrain.isActive) _danceBrain.EnableMovement(true);
        _danceBrain.EnableMovement(true);
        animator.ResetTrigger("Pulse");
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
    private void SetBeatDuration()
    {
        currentBeatOnPlayer = BeatManager.Instance.quarterBeatDuration;
        animator.enabled = true;
        animator.SetFloat("Beat",(float)(1/currentBeatOnPlayer));
    }
    
    public void Activate()
    {
        allowInput = true;   
    }

    public void Disactivate()
    {
        allowInput = false;
    }

    public void AnimationFeedbackEvent(string eventName)
    {
        playerFeedbackEvents.FirstOrDefault(x=> x.name==eventName)?.feedbackEvent?.Invoke();
    }
}
