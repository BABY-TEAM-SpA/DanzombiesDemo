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
    [SerializeField] private AnimatorOverrideController[] animatorOverrideControllers;
    private double currentBeatOnPlayer = 0d;
    private bool isDirectionPulsed;
    private DanceDirection isDancePulsed;

    public List<AnimationFeedback> playerFeedbackEvents = new List<AnimationFeedback>();
    
    private void Start()
    {
        SetAnimatorOverrideDirection();
    }
    
    
    /*
    public void OnPulse()
    {
        animator.ResetTrigger("Pulse");
        animator.SetTrigger("Pulse");

    }*/
    
    public void OnMoving(Vector3 direction)
    {
        bool moving = direction != Vector3.zero;
        animator.SetBool("RightLooking", _danceBrain.isRightLooking);
        if (moving)
        {
            animator.ResetTrigger("Pulse");
        }
        animator.SetBool("Walking", moving);
        animator.SetFloat("WalkingSpeed", direction.magnitude);
    }
    
    public void OnDanceBegin(DanceStep step)
    {
        Debug.Log("OnDanceBegin");
        _danceBrain?.EnableMovement(false);
        _danceBrain.OnDance(step);
        animator.Play(step.ToString(), 0,0f);
    }
    public void OnStandAction()
    {
        if(_danceBrain.isActive) _danceBrain.EnableMovement(true);
        //_danceBrain.OnDance(DanceStep.None);
        animator.ResetTrigger("Pulse");
    }

    public void SetAnimatorOverrideDirection()
    {
        bool isRight = _danceBrain.isRightLooking;
        animator.SetBool("RightLooking", isRight);
        animator.runtimeAnimatorController = animatorOverrideControllers[isRight?0:1];
    }
    private void SetBeatDuration()
    {
        currentBeatOnPlayer = AudioManager.Instance.currentSongPlaying.beatDuration;
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
