using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum ExpressionType {Normal,Angry}
[Serializable]
public class DancerExpression
{
    public ExpressionType expressionType;
    public AnimatorOverrideController alpha;
    public AnimatorOverrideController beta;
    public UnityEvent OnReaction;
}

public abstract class DanceBrain : Dancer
{
    #region [VARIABLES]
    [SerializeField] protected bool debug;
    [SerializeField]
    private bool ActivateOnStart =false;
    [SerializeField] protected PlayerMovementController movCtrl;
    [SerializeField] protected DanceAnimatorController danceAnimCtrl;
    [SerializeField] protected BeatReciever beatReciever;
    public bool isLeftLooking;
    private bool enableMovement=true;
    
    [SerializeField] List<DancerExpression> dancerExpressions = new List<DancerExpression>();
    public event Action<bool> OnDirectionChanged;

    #endregion

    #region [METHODS]
    public void EnableMovement(bool isON = false)
    {
        enableMovement = isON;
    }
    public void ResetScriptedMovement() => movCtrl?.StopScriptedMovement();
    
    public void Start()
    {
        DancerExpression expression = dancerExpressions.First();
        danceAnimCtrl.SetExpression(expression.alpha, expression.beta);
        if(ActivateOnStart)ActivateEntity(ActivateOnStart);
    }

    public virtual void ActivateEntity(bool  activate)
    {
        
        if (activate) movCtrl?.StopScriptedMovement();
        beatReciever?.SetActive(activate);
    }

    public void Move(Vector2 inputMovementDirection)
    {
        if (!enableMovement) inputMovementDirection = Vector2.zero;
        movCtrl?.SetDirection(inputMovementDirection);
    }
    public void OnMoving(Vector3 direction)
    {
        danceAnimCtrl?.AnimateOnMoving(direction);
    }

    public void SetBodyDirection(float value)
    {
        if (Math.Abs(value) > 0.5)
        {
            bool isLeft = value < 0;
            if (isLeft != isLeftLooking && value != 0)
            {
                isLeftLooking = isLeft;
                if (TryGetComponent(out Animator animator))  animator.SetBool("isLeftLooking", isLeft);
                danceAnimCtrl.SetAnimatorOverrideDirection();
                OnDirectionChanged?.Invoke(isLeft);
            }
        }
    }
    
    public override void React(ExpressionType exp)
    {
        DancerExpression expression = dancerExpressions.FirstOrDefault((expression) => expression.expressionType == exp);
        if (expression != null) { 
            danceAnimCtrl.SetExpression(expression.alpha, expression.beta);
            expression.OnReaction?.Invoke();
        }
    }
    #endregion
    
}
