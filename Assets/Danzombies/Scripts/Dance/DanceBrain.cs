using System;
using UnityEngine;

public abstract class DanceBrain : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] protected bool debug;
    [SerializeField]
    private bool ActivateOnStart;
    public bool isActive { get; set; } = true;
    [SerializeField] protected PlayerMovementController movCtrl;
    [SerializeField] protected DanceAnimatorController danceAnimCtrl;
    [SerializeField] protected BeatReciever beatReciever;
    public bool isLeftLooking;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public event Action<bool> OnDirectionChanged;
    #endregion

    #region [METHODS]
    public void EnableMovement(bool isON=false)
    {
        movCtrl?.StopScriptedMovement();
        if (isON) movCtrl?.EnableInput();
        else movCtrl?.DisableInput();
    }
    public void EnableDance(bool isON=false)
    {
        if (isON) danceAnimCtrl?.Activate();
        else danceAnimCtrl?.Disactivate();
    }
    public void Start()
    {
        if(ActivateOnStart)ActivateEntity(ActivateOnStart);
    }

    public virtual void ActivateEntity(bool  activate)
    {
        isActive = activate;
        EnableMovement(activate);
        EnableDance(activate);
        beatReciever.SetActive(activate);
    }

    public abstract void OnDance(DanceStep step);

    public void OnMoving(Vector3 direction)
    {
        danceAnimCtrl.OnMoving(direction);
    }

    public void SetBodyDirection(float value)
    {
        if (Math.Abs(value) > 0.5)
        {
            bool isLeft = value < 0;
            if (isLeft != isLeftLooking && value != 0)
            {
                isLeftLooking = isLeft;
                if (TryGetComponent(out Animator animator))
                    animator.SetBool("isLeftLooking", isLeft);
                danceAnimCtrl.SetAnimatorOverrideDirection();
                OnDirectionChanged?.Invoke(isLeft);
            }
        }
        
    }
    #endregion
}
