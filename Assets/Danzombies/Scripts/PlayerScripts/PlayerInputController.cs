using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    
    public bool allowMoveInput;
    public void EnableMoveInput() => allowMoveInput = true;
    public void DisableMoveInput()
    {
        allowMoveInput = false;
        inputMovementDirection = Vector2.zero;
        _playerManager.Move(inputMovementDirection);
    }

    public bool allowDanceInput;
    public void EnableDanceInput() => allowDanceInput = true;
    public void DisableDanceInput() => allowDanceInput = false;
    [SerializeField, Range(0.5f,1f)] float margen = 0.5f;
    public bool allowInteractInput;
    public void EnableInteractInput() => allowInteractInput = true;
    public void DisableInteractInput() => allowInteractInput = false;
    
    [SerializeField] PlayerManager _playerManager;
    DanceLean inputDanceLean;
    DanceDirection inputDanceDirection;
    private Vector2 inputMovementDirection;
    
    
    #region [EVENTS]
    public void OnMoveAction(InputAction.CallbackContext context)
    {
        if (!allowMoveInput) return;
        if (context.performed) inputMovementDirection = context.ReadValue<Vector2>();
        if (context.canceled) inputMovementDirection = Vector2.zero;
        _playerManager.Move(inputMovementDirection);
    }

    public void OnSprintAction(InputAction.CallbackContext context)
    {
        if (!allowMoveInput) return;
        if (context.performed) _playerManager.InputSprint(true);
        //else if (context.canceled) SetSpeed(walkingSpeed);
    }
    #endregion
    
    public void OnDirectionButtonAction(InputAction.CallbackContext context)
    {
        if (!allowDanceInput) return;
        if (context.performed)
        {
            Vector2 value = context.ReadValue<Vector2>();
            if (value.x > margen) inputDanceDirection = DanceDirection.East;
            else if (value.x < -margen) inputDanceDirection = DanceDirection.West;
            else if (value.y >  margen) inputDanceDirection = DanceDirection.North;
            else if (value.y < -margen) inputDanceDirection = DanceDirection.South;
            _playerManager.InputDance(inputDanceLean,inputDanceDirection);
        }
        if (context.canceled)
        {
            inputDanceDirection = DanceDirection.None;
        }
    }

    public void OnLeanButtonAction(InputAction.CallbackContext context)
    {
        if (!allowDanceInput) return;
        if (context.started)
        {
            float valor = context.ReadValue<float>();
            inputDanceLean = valor>0? DanceLean.R : DanceLean.L;
            _playerManager.InputDance(inputDanceLean, inputDanceDirection);
        }
        if (context.canceled)
        { 
            inputDanceLean = DanceLean.None;
        }
    }
    
    public void OnInteractEvent(InputAction.CallbackContext context)
    {
        if (!allowInteractInput) return;
        //if (context.performed) Interact();
    }
    

    
}
