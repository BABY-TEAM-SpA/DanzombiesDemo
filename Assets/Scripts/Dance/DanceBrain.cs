using UnityEngine;

public abstract class DanceBrain : MonoBehaviour
{
    [SerializeField] protected PlayerMovementController playerMovCtrl;
    [SerializeField] protected PlayerAnimatorController playerAnimCtrl;
    public bool isRightLooking;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void EnableMovement(bool isON=false)
    {
        if (isON) playerMovCtrl?.EnableMovement();
        else playerMovCtrl?.DisableMovement();
    }

    public virtual void OnDance(DanceStep step)
    {
        
    }

    public void OnMoving(bool moving)
    {
        playerAnimCtrl.OnMoving(moving);
    }

    public void SetBodyDirection(float value)
    {
        bool isRight = value > 0;
        if(isRight != isRightLooking && value!=0)
        {
            isRightLooking = isRight;
            playerAnimCtrl.SetAnimatorOverrideDirection(isRight);
        }
    }
    
}
