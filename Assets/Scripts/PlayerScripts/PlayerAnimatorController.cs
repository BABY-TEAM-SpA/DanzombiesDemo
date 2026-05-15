using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimatorController : DanceAnimatorController
{
    [SerializeField, Range(0.5f,1f)] float margen = 0.5f;
    DanceDirection currentDirection;
    private bool directionInput;
    DanceLean currentLean;
    bool leanInput;
    
    public void OnDirectionButtonPressed(InputAction.CallbackContext context)
    {
        if (allowInput && _danceBrain.isActive)
        {
            if (context.performed)
            {
                Vector2 value = context.ReadValue<Vector2>();
                directionInput = value != Vector2.zero;
                if (value.x > margen)
                    currentDirection = DanceDirection.East;
                else if (value.x < -margen)
                    currentDirection = DanceDirection.West;
                else if (value.y > margen)
                    currentDirection = DanceDirection.North;
                else if (value.y < -margen)
                    currentDirection = DanceDirection.South;
                TryMakeDanceStep();
            }
        }
        if (context.canceled)
        {
            currentDirection = DanceDirection.None;
            directionInput = false;
            danceTriggered = false;
        }
        
    }

    public void OnLeanButtonPressed(InputAction.CallbackContext context)
    {
        if (allowInput && _danceBrain.isActive)
        {
            
            if (context.started)
            {
                float valor = context.ReadValue<float>();
                leanInput = valor != 0;
                if(leanInput) currentLean = valor>0?DanceLean.R:DanceLean.L;
                TryMakeDanceStep();
            }
               
        }
        if (context.canceled)
        {
           currentLean = DanceLean.None;
           leanInput = false;
           danceTriggered = false;
        } 
    }

    bool danceTriggered;

    private void TryMakeDanceStep()
    {
        if(leanInput && directionInput && !danceTriggered)
        {
            Debug.Log("Here");
            danceTriggered=true;
            DanceStep step = Enum.Parse<DanceStep>( currentLean + "_" + currentDirection );
            OnDanceBegin(step);
        }
    }
    
}
