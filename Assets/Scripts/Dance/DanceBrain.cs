using UnityEngine;

public abstract class DanceBrain : MonoBehaviour
{
    public bool isActive = true;
    [SerializeField] protected PlayerMovementController playerMovCtrl;
    [SerializeField] protected PlayerAnimatorController playerAnimCtrl;
    [SerializeField] protected BeatReciever beatReciever;
    public bool isRightLooking;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void EnableMovement(bool isON=false)
    {
        if (isON) playerMovCtrl?.EnableMovement();
        else playerMovCtrl?.DisableMovement();
    }
    public void EnableDance(bool isON=false)
    {
        if (isON) playerAnimCtrl?.Activate();
        else playerAnimCtrl?.Disactivate();
    }

    public virtual void OnDance(DanceStep step)
    {
        
    }

    public void OnMoving(Vector3 direction)
    {
        playerAnimCtrl.OnMoving(direction);
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
