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

[Serializable]
public enum DanceDirection{
    None,
    North,
    South,
    West,
    East
}
[Serializable]
public class TalkingExpressionData
{
    public DialogExpression expression;
    public AnimationClip alphaIntro;
    public AnimationClip betaIntro;
    public AnimationClip alphaLoop;
    public AnimationClip betaLoop;
    public AnimationClip alphaEnd;
    public AnimationClip betaEnd;
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
    private bool isWalking;
    
    public List<TalkingExpressionData> talkingExpressions = new List<TalkingExpressionData>();
    private Action onTalkingEnd;
    
    
    private void Start()
    {
        SetAnimatorOverrideDirection();
    }
    
    public void AnimateOnMoving(Vector3 velocity)
    {
        isWalking = velocity != Vector3.zero;
        animator?.SetBool("LeftLooking", _danceBrain.isLeftLooking);
        animator?.SetBool("Walking", isWalking);
        animator?.SetFloat("WalkingSpeed", velocity.magnitude);
    }
    
    public void OnDanceBegin(DanceStep step)
    {
        if (step == DanceStep.None) return; 
        _danceBrain?.EnableMovement(false);
        animator.Play(step.ToString(), 0,0f);
    }

    public void OnDanceBegin(DanceStep step, BeatManager.BeatType beatType)
    {
        if (step == DanceStep.None && beatType == BeatManager.BeatType.FullBeat && !isWalking) animator.Play("None" );
        else OnDanceBegin(step);
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


    public void Talking(DialogExpression expression)
    {
        TalkingExpressionData expressionData = talkingExpressions.FirstOrDefault(x => x.expression == expression);
        if(expressionData == null) return;
        alphaOverrider["BaseTalkingEnter"] = expressionData.alphaIntro;
        betaOverrider["BaseTalkingEnter"] = expressionData.betaIntro;
        animator.Play("TalkingEnter");
        alphaOverrider["BaseTalkingLoop"] = expressionData.alphaLoop;
        betaOverrider["BaseTalkingLoop"] = expressionData.betaLoop;
        alphaOverrider["BaseTalkingExit"] = expressionData.alphaEnd;
        betaOverrider["BaseTalkingExit"] = expressionData.betaEnd;
        onTalkingEnd = ()=>animator.Play("TalkingExit");
    }

    public void TalkingEnd()
    {
        onTalkingEnd?.Invoke();
        onTalkingEnd = null;
        
    }
    
    
    
}
