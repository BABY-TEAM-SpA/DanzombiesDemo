using UnityEngine;

public class DanceBrain : MonoBehaviour
{
    [SerializeField] protected PlayerMovementController playerMovCtrl;
    [SerializeField] protected PlayerAnimatorController playerAnimCtrl;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void EnableMovement(bool isON=false)
    {
        if (isON) playerMovCtrl?.EnableMovement();
        else playerMovCtrl?.DisableMovement();
    }

    public virtual void OnDance(DanceStep step)
    {
    }

    public void OnMoving(float speed)
    {
        playerAnimCtrl.OnMoving(speed);
    }

    public void SetBodyDirection(bool isRight)
    {
        playerAnimCtrl.SetAnimatorOverrideDirection(isRight);
    }
}
