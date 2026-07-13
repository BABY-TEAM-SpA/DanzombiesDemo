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
    [SerializeField] private bool isTutorial;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] SpriteRenderer outlineRenderer;

    private void Update()
    {
        if (outlineRenderer == null) return;
        if (outlineRenderer.sprite == renderer.sprite) return;
        outlineRenderer.sprite = renderer.sprite;
    }

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
                if(isTutorial)animator.SetBool("PrepareDance",leanInput);
                if(leanInput) currentLean = valor>0?DanceLean.R:DanceLean.L;
                TryMakeDanceStep();
            }
               
        }
        if (context.canceled)
        { 
            if(isTutorial)animator.SetBool("PrepareDance",false);
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
            if(isTutorial)animator.SetBool("PrepareDance",false);
            danceTriggered=true;
            DanceStep step = Enum.Parse<DanceStep>( currentLean + "_" + currentDirection );
            OnDanceBegin(step);
        }
    }
    
}
