using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] PlayerManager _playerManager;

    /// <summary>
    /// Para simular que el jugador soltó todos los inputs, para poder dejar a Greg en un estado de Idle.
    /// </summary>
    public void ReleaseInputs()
    {
        inputMovementDirection = Vector2.zero;
        _playerManager.Move(inputMovementDirection);
        _playerManager.InputSprint(false);

        inputDanceLean = DanceLean.None;
        inputDanceDirection = DanceDirection.None;
        _playerManager.InputDance(inputDanceLean, inputDanceDirection);
    }

    #region [MOVEMENT]
    public bool allowMoveInput;
    private Vector2 inputMovementDirection;

    #region API
    public void EnableMoveInput() => allowMoveInput = true;
    public void DisableMoveInput()
    {
        allowMoveInput = false;
        inputMovementDirection = Vector2.zero;
        _playerManager.Move(inputMovementDirection);
    }
    #endregion

    #region Input Events
    public void OnMoveAction(InputAction.CallbackContext context)
    {
        if (!allowMoveInput)
            return;

        if (context.performed) inputMovementDirection = context.ReadValue<Vector2>().normalized;
        if (context.canceled) inputMovementDirection = Vector2.zero;
        _playerManager.Move(inputMovementDirection);
    }

    public void OnSprintAction(InputAction.CallbackContext context)
    {
        if (!allowMoveInput)
            return;

        if (context.performed) _playerManager.InputSprint(true);
        else if (context.canceled) _playerManager.InputSprint(false);
    }
    #endregion
    #endregion

    #region [DANCE]
    public bool allowDanceInput;
    [SerializeField, Range(0.5f, 1f)] private float margin = 0.5f;
    private DanceLean inputDanceLean;
    private DanceDirection inputDanceDirection;

    #region API
    public void EnableDanceInput() => allowDanceInput = true;
    public void DisableDanceInput() => allowDanceInput = false;
    #endregion

    #region Input Events
    public void OnDirectionButtonAction(InputAction.CallbackContext context)
    {
        if (!allowDanceInput)
            return;

        if (context.performed)
        {
            Vector2 value = context.ReadValue<Vector2>();
            if (value.x > margin) inputDanceDirection = DanceDirection.East;
            else if (value.x < -margin) inputDanceDirection = DanceDirection.West;
            else if (value.y > margin) inputDanceDirection = DanceDirection.North;
            else if (value.y < -margin) inputDanceDirection = DanceDirection.South;
            _playerManager.InputDance(inputDanceLean, inputDanceDirection);
        }
        if (context.canceled)
        {
            inputDanceDirection = DanceDirection.None;
        }
    }

    public void OnLeanButtonAction(InputAction.CallbackContext context)
    {
        if (!allowDanceInput)
            return;

        if (context.started)
        {
            float valor = context.ReadValue<float>();
            inputDanceLean = valor > 0 ? DanceLean.R : DanceLean.L;
            _playerManager.InputDance(inputDanceLean, inputDanceDirection);
        }
        if (context.canceled)
        {
            inputDanceLean = DanceLean.None;
        }
    }
    #endregion
    #endregion

    #region [INTERACTION]
    public bool allowInteractInput;

    #region API
    public void EnableInteractInput() => allowInteractInput = true;
    public void DisableInteractInput() => allowInteractInput = false;
    #endregion

    #region Input Events
    public void OnInteractEvent(InputAction.CallbackContext context)
    {
        if (!allowInteractInput)
            return;

        if (context.performed)
            _playerManager.InputInteract();
    }
    #endregion
    #endregion
}
