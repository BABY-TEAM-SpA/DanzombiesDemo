using System;
using UnityEngine;

public abstract class DanceBrain : MonoBehaviour
{
    [SerializeField] protected bool debug;
    public bool isActive { get; set; } = true;
    [SerializeField] protected PlayerMovementController movCtrl;
    [SerializeField] protected DanceAnimatorController danceAnimCtrl;
    [SerializeField] protected BeatReciever beatReciever;
    public bool isRightLooking{ get; set; } 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void EnableMovement(bool isON=false)
    {
        if (isON) movCtrl?.EnableMovement();
        else movCtrl?.DisableMovement();
    }
    public void EnableDance(bool isON=false)
    {
        if (isON) danceAnimCtrl?.Activate();
        else danceAnimCtrl?.Disactivate();
    }

    public abstract void OnDance(DanceStep step);

    public void OnMoving(Vector3 direction)
    {
        danceAnimCtrl.OnMoving(direction);
    }

    public void SetBodyDirection(float value)
    {
        bool isRight = value > 0;
        if(isRight != isRightLooking && value!=0)
        {
            isRightLooking = isRight;
            danceAnimCtrl.SetAnimatorOverrideDirection();
        }
    }
    
}
