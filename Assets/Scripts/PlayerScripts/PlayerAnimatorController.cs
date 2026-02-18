
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum DanceState{
	None,
	North,
	South,
	West,
	East
}

public class PlayerAnimatorController : MonoBehaviour
{
  
    [SerializeField] private DanceBrain _danceBrain;
    [SerializeField] private bool allowInput = false;
    [SerializeField] public Animator animator;
    //[SerializeField] private SpriteRenderer renderer;
    [SerializeField] private AnimatorOverrideController[] animatorOverrideControllers;
    private double currentBeatOnPlayer = 0d;
	private bool isDirectionPulsed;
	private DanceState isDancePulsed;

    private void Start()
    {
        SetAnimatorOverrideDirection(_danceBrain.isRightLooking);
    }
    
    public void OnNorthButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Dance");
        if (allowInput && _danceBrain.isActive)
        {
            animator.ResetTrigger("Pulse");
            if (context.started)
            {
                animator.SetBool("DanceStepN",true);
            }
            if (context.performed)
            {
                animator.SetTrigger("Dance");
            }
        }
        if (context.canceled)
        {
            animator.SetBool("DanceStepN",false);
            //animator.ResetTrigger("Dance");
        }
        
    }

    public void OnSouthButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Dance");
        animator.ResetTrigger("Pulse");
        if (allowInput && _danceBrain.isActive)
        {
            if (context.started)
            {
                animator.SetBool("DanceStepS",true);
            }
            if (context.performed)
            {
                animator.SetTrigger("Dance");
            }
        }
        if (context.canceled)
        {
            animator.SetBool("DanceStepS",false);
            //animator.ResetTrigger("Dance");
        }
    }

    public void OnWestButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Dance");
        animator.ResetTrigger("Pulse");
        if (allowInput && _danceBrain.isActive)
        {
            animator.ResetTrigger("Pulse");
            if (context.started)
            {
                animator.SetBool("DanceStepW",true);
            }
            if (context.performed)
            {
                animator.SetTrigger("Dance");
            }
        }
        if (context.canceled)
        {
            animator.SetBool("DanceStepW",false);
            //animator.ResetTrigger("Dance");
        }
    }

    public void OnEastButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Dance");
        animator.ResetTrigger("Pulse");
        if (allowInput && _danceBrain.isActive)
        {
            if (context.started)
            {
                animator.SetBool("DanceStepE",true);
            }
            if (context.performed)
            {
                animator.SetTrigger("Dance");
            }
        }
        if (context.canceled)
        {
            animator.SetBool("DanceStepE",false );
            //animator.ResetTrigger("Dance");
        }
       
    }

    public void OnLeftUPButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Dance");
        animator.ResetTrigger("Pulse");
        if (allowInput && _danceBrain.isActive)
        {
            
            if (context.started)
            {
                animator.SetBool("RightDanceDir",false);
                animator.SetBool("LeftDanceDir",true);
            }
            if (context.performed)
            {
                animator.SetTrigger("Dance");
            }
               
        }
        if (context.canceled)
        {
            animator.SetBool("LeftDanceDir",false );
        } 
    }

    public void OnRightUPButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Dance");
        animator.ResetTrigger("Pulse");
        if (allowInput && _danceBrain.isActive)
        {
            
            if (context.started)
            {
                animator.SetBool("LeftDanceDir",false);
                animator.SetBool("RightDanceDir",true);
            }
            if (context.performed)
            {
                animator.SetTrigger("Dance");
            }
        }
        if (context.canceled)
        {
            animator.SetBool("RightDanceDir",false);
        }
    }
    /*
    public void OnPulse()
    {
        animator.ResetTrigger("Pulse");
        animator.SetTrigger("Pulse");
        
    }*/
    public void OnMoving(bool moving)
    {
        bool walking = moving;//>0.1;
        if (walking)
        {
            animator.ResetTrigger("Pulse");
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
        if(_danceBrain.isActive) _danceBrain.EnableMovement(true);
        _danceBrain.OnDance(DanceStep.None);
        animator.ResetTrigger("Pulse");
        animator.ResetTrigger("Dance");
    }

    public void SetAnimatorOverrideDirection(bool isRight)
    {
        animator.runtimeAnimatorController = animatorOverrideControllers[isRight?0:1];
    }
    private void SetBeatDuration()
    {
        currentBeatOnPlayer = AudioManager.Instance.beatDuration;
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
    
    
}
