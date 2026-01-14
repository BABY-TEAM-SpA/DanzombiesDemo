
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerAnimatorController : MonoBehaviour
{
    
    [SerializeField] private PlayerMovementController playerMovCtrl;
    [SerializeField] private float danceWalkingSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private AnimatorOverrideController[] animatorOverrideControllers;
    private bool moving =false;
    private bool isDancing = false;
    [SerializeField] private bool ShouldEnter= false;
    
    [Header("Events")]
    public UnityAction<DanceStep> OnDance;
    public UnityAction OnDanceEnd;
    
    private void Start()
    {
        animator.runtimeAnimatorController = animatorOverrideControllers[0];
        if(ShouldEnter)animator.SetTrigger("Enter");
    }

    void Update()
    {
        moving = false;
        if (playerMovCtrl != null)
        {
            moving = playerMovCtrl.isWalking;
            if(renderer!=null)renderer.transform.localRotation = Quaternion.Euler(0,(playerMovCtrl.velocity.x<0 &&!isDancing)? 180f : 0f,0f); 
        }
        OnMoving(moving);
    }
    
    public void OnNorthButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Idle");
        if (context.performed)
        {
            animator.SetBool("DanceStepN", true);
            //animator.SetTrigger("Dance");
        }

        if (context.canceled)
        {
            animator.SetBool("DanceStepN", false);
            //animator.ResetTrigger("Dance");
        }
    }

    public void OnSouthButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Idle");
        if (context.performed)
        {
            animator.SetBool("DanceStepS", true);
            //animator.SetTrigger("Dance");
        }

        if (context.canceled)
        {
            animator.SetBool("DanceStepS", false);
            //animator.ResetTrigger("Dance");
        }
    }

    public void OnWestButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Idle");
        if (context.performed)
        {
            animator.SetBool("DanceStepW", true);
            //animator.SetTrigger("Dance");
        }

        if (context.canceled)
        {
            animator.SetBool("DanceStepW", false);
            //animator.ResetTrigger("Dance");
        }
    }

    public void OnEastButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Idle");
        if (context.performed)
        {
            animator.SetBool("DanceStepE", true);
            //animator.SetTrigger("Dance");
        }

        if (context.canceled)
        {
            animator.SetBool("DanceStepE", false);
            //animator.ResetTrigger("Dance");
        }
    }

    public void OnLeftUPButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Idle");
        if (context.started)
        {
            animator.SetBool("RightLook", false);
        }
        if (context.performed)
        {
            animator.SetTrigger("Dance");
        }
        if (context.canceled)
        {
            
            animator.ResetTrigger("Dance");
        }
    }

    public void OnRightUPButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Idle");
        if (context.started)
        {
            animator.SetBool("RightLook", true);
        }
        if (context.performed)
        {
            animator.SetTrigger("Dance");
        }

        if (context.canceled)
        {
           
           animator.ResetTrigger("Dance");
        }
    }
    
    public void OnIDLE()
    {
        animator.ResetTrigger("Idle");
        animator.SetTrigger("Idle");
        
    }
    private void OnMoving(bool moving)
    {
        if (moving)
        {
            animator.ResetTrigger("Idle");
        }
        animator.SetBool("isWalking", moving);
    }
    
    public void OnDanceBegin(int danceIndex)
    {
        playerMovCtrl.SetSpeed(danceWalkingSpeed);
        isDancing = true;
        DanceStep step = (DanceStep)danceIndex;
        OnDance?.Invoke(step);
        //playerMovCtrl.EventDance(step);
    }
    public void OnStandAction()
    {
        isDancing = false;
        playerMovCtrl.SetSpeed();
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Dance");
        OnDanceEnd?.Invoke();   
    }
}
