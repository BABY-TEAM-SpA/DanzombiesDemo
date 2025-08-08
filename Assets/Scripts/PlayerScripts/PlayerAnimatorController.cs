
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField] public UnityAction OnDanceEnd;
    [SerializeField] private PlayerMovementController playerMovCtrl;
    [SerializeField] private float danceWalkingSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private AnimatorOverrideController[] animatorOverrideControllers;
    private bool moving =false;
    private bool isDancing = false;
    [SerializeField] private bool ShouldEnter= false;

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
            animator.SetTrigger("DanceNorth");
        }

        if (context.canceled)
        {
            animator.ResetTrigger("DanceNorth");
        }
    }

    public void OnSouthButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Idle");
        if (context.performed)
        {
            animator.SetTrigger("DanceSouth");
        }
        if (context.canceled)
        {
            animator.ResetTrigger("DanceSouth");
        }
    }

    public void OnWestButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Idle");
        if (context.performed)
        {
            
            animator.SetTrigger("DanceWest");
        }
        if (context.canceled)
        {
            animator.ResetTrigger("DanceWest");
        }
    }

    public void OnEastButtonPressed(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("Idle");
        if (context.performed)
        {
            
            animator.SetTrigger("DanceEast");
        }
        if (context.canceled)
        {
            animator.ResetTrigger("DanceEast");
        }
    }

    public void OnLeftUPButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetBool("LeftTrigger", true);
        }

        if (context.canceled)
        {
            animator.SetBool("LeftTrigger", false); 
        }
    }

    public void OnRightUPButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetBool("RightTrigger", true);
        }

        if (context.canceled)
        {
           animator.SetBool("RightTrigger", false); 
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
    private void OnDanceBegin()
    {
        playerMovCtrl.SetSpeed(danceWalkingSpeed);
        isDancing = true;
    }
    public void OnStandAction()
    {
        Debug.Log("Stand");
        
        isDancing = false;
        playerMovCtrl.SetSpeed();
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("DanceNorth");
        animator.ResetTrigger("DanceSouth");
        animator.ResetTrigger("DanceWest");
        animator.ResetTrigger("DanceEast");
        OnDanceEnd?.Invoke();   
    }
    
}
