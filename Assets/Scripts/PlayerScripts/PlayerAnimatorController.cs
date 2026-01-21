
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField] private DanceBrain _danceBrain;
    [SerializeField] public Animator animator;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private AnimatorOverrideController[] animatorOverrideControllers;

    private void Start()
    {
        SetAnimatorOverrideDirection(_danceBrain.isRightLooking);
    }
    
    public void OnNorthButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Idle");
        if (context.started)
        {
            animator.SetBool("DanceStepS",false);
            animator.SetBool("DanceStepW",false);
            animator.SetBool("DanceStepE",false);
            animator.SetBool("DanceStepN",true);
        }
        if (context.performed)
        {
            animator.SetTrigger("Dance");
        }

        if (context.canceled)
        {
            animator.SetBool("DanceStepN",false);
            //animator.ResetTrigger("Dance");
        }
    }

    public void OnSouthButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Idle");
        if (context.started)
        {
            animator.SetBool("DanceStepN",false);
            animator.SetBool("DanceStepW",false);
            animator.SetBool("DanceStepE",false);
            animator.SetBool("DanceStepS",true);
        }
        if (context.performed)
        {
            animator.SetTrigger("Dance");
        }

        if (context.canceled)
        {
            animator.SetBool("DanceStepS",false);
            //animator.ResetTrigger("Dance");
        }
    }

    public void OnWestButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Idle");
        if (context.started)
        {
            animator.SetBool("DanceStepN",false);
            animator.SetBool("DanceStepS",false);
            animator.SetBool("DanceStepE",false);
            animator.SetBool("DanceStepW",true);
        }
        if (context.performed)
        {
            animator.SetTrigger("Dance");
        }

        if (context.canceled)
        {
            animator.SetBool("DanceStepW",false);
            //animator.ResetTrigger("Dance");
        }
    }

    public void OnEastButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Idle");

        if (context.started)
        {
            animator.SetBool("DanceStepN",false);
            animator.SetBool("DanceStepS",false);
            animator.SetBool("DanceStepW",false);
            animator.SetBool("DanceStepE",true);
        }
        if (context.performed)
        {
            animator.SetTrigger("Dance");
        }

        if (context.canceled)
        {
            animator.SetBool("DanceStepE",false );
            //animator.ResetTrigger("Dance");
        }
    }

    public void OnLeftUPButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Idle");
        if (context.started)
        {
            animator.SetBool("RightDanceDir",false);
            animator.SetBool("LeftDanceDir",true);
        }
        if (context.performed)
        {
            animator.SetTrigger("Dance");
        }
        if (context.canceled)
        {
            animator.SetBool("LeftDanceDir",false );
        }
    }

    public void OnRightUPButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Idle");
        if (context.started)
        {
            animator.SetBool("LeftDanceDir",false);
            animator.SetBool("RightDanceDir",true);
        }
        if (context.performed)
        {
            animator.SetTrigger("Dance");
        }

        if (context.canceled)
        {
            animator.SetBool("RightDanceDir",false);
            
        }
    }
    
    public void OnIDLE()
    {
        animator.ResetTrigger("Idle");
        animator.SetTrigger("Idle");
        
    }
    public void OnMoving(float moving)
    {
        bool walking = moving>0.1;
        if (walking)
        {
            animator.ResetTrigger("Idle");
        }
        animator.SetBool("Walking", walking);
    }
    
    public void OnDanceBegin(int danceIndex)
    {
        _danceBrain?.EnableMovement(false);
        DanceStep step = (DanceStep)danceIndex;
        _danceBrain.OnDance(step);

        animator.ResetTrigger("Dance");
    }
    public void OnStandAction()
    {
        _danceBrain.EnableMovement(true);
        _danceBrain.OnDance(DanceStep.None);
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Dance");
    }

    public void SetAnimatorOverrideDirection(bool isRight)
    {
        animator.runtimeAnimatorController = animatorOverrideControllers[isRight?0:1];
    }
}
