using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] PlayerManager _playerManager;
    [SerializeField] AllowedPlayerInputs allowedInputs;

    private float elapsed;
    private float recoveryTime;
    private AllowedPlayerInputs allowedInputsBeforeDisabling;
    #endregion

    #region [UNITY]
    private void Update()
    {
        if (recoveryTime > 0f)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= recoveryTime)
            {
                elapsed = 0f;
                recoveryTime = 0f;
                EnableMoveInput();
            }
        }
    }
    #endregion

    #region [GLOBAL]
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

    public void EnableAllInputs()
    {
        EnableMoveInput();
        EnableDanceInput();
        EnableInteractInput();
    }
    public void RecoverInputs()
    {
        if (allowedInputsBeforeDisabling == null)
            return;

        if (allowedInputsBeforeDisabling.movement) EnableMoveInput();
        if (allowedInputsBeforeDisabling.dance) EnableDanceInput();
        if (allowedInputsBeforeDisabling.interaction) EnableInteractInput();
        allowedInputsBeforeDisabling = null;
    }
    public void DisableAllInputs()
    {
        allowedInputsBeforeDisabling = new AllowedPlayerInputs(allowedInputs);

        DisableMoveInput();
        DisableDanceInput();
        DisableInteractInput();
    }
    #endregion

    #region [MOVEMENT]
    [Header("Movement")]
    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference sprintAction;
    private Vector2 inputMovementDirection;

    #region API
    public void EnableMoveInput()
    {
        allowedInputs.movement = true;

        inputMovementDirection = moveAction.action.ReadValue<Vector2>().normalized;
        _playerManager.Move(inputMovementDirection);
        _playerManager.InputSprint(sprintAction.action.IsPressed());
    }

    public void DisableMoveInput()
    {
        allowedInputs.movement = false;
        inputMovementDirection = Vector2.zero;
        _playerManager.Move(inputMovementDirection);
        _playerManager.InputSprint(false);
    }

    public void DisableMoveForSeconds(float seconds)
    {
        recoveryTime = seconds;
        DisableMoveInput();
    }
    #endregion

    #region Input Events
    public void OnMoveAction(InputAction.CallbackContext context)
    {
        if (!allowedInputs.movement)
            return;

        if (context.performed) inputMovementDirection = context.ReadValue<Vector2>().normalized;
        if (context.canceled) inputMovementDirection = Vector2.zero;
        _playerManager.Move(inputMovementDirection);
    }

    public void OnSprintAction(InputAction.CallbackContext context)
    {
        if (!allowedInputs.movement)
            return;

        if (context.performed) _playerManager.InputSprint(true);
        else if (context.canceled) _playerManager.InputSprint(false);
    }
    #endregion
    #endregion

    #region [DANCE]
    [Header("Dance")]
    [SerializeField, Range(0.5f, 1f)] private float margin = 0.5f;
    private DanceLean inputDanceLean;
    private DanceDirection inputDanceDirection;

    #region API
    public void EnableDanceInput() => allowedInputs.dance = true;
    public void DisableDanceInput() => allowedInputs.dance = false;
    
    public void EnableDisableDanceNorth(bool isOn) => allowedInputs.danceInputs.northDance = isOn;
    public void EnableDisableDanceSouth(bool isOn) => allowedInputs.danceInputs.southDance = isOn;
    public void EnableDisableDanceEast(bool isOn) => allowedInputs.danceInputs.eastDance = isOn;
    public void EnableDisableDanceWest(bool isOn) => allowedInputs.danceInputs.westDance = isOn;
    #endregion

    #region Input Events
    public void OnDirectionButtonAction(InputAction.CallbackContext context)
    {
        if (!allowedInputs.dance)
            return;

        if (context.performed)
        {
            Vector2 value = context.ReadValue<Vector2>();
            DanceDirection input = DanceDirection.None;
            if (value.x > margin && allowedInputs.danceInputs.eastDance) input = DanceDirection.East;
            else if (value.x < -margin && allowedInputs.danceInputs.westDance) input = DanceDirection.West;
            else if (value.y > margin && allowedInputs.danceInputs.northDance) input = DanceDirection.North;
            else if (value.y < -margin && allowedInputs.danceInputs.southDance) input = DanceDirection.South;
            if(input != DanceDirection.None && inputDanceDirection != input)
            {
                inputDanceDirection = input;
                _playerManager.InputDance(inputDanceLean, inputDanceDirection);
            }
        }
        if (context.canceled)
        {
            inputDanceDirection = DanceDirection.None;
        }
    }

    public void OnLeanButtonAction(InputAction.CallbackContext context)
    {
        if (!allowedInputs.dance)
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
    #region API
    public void EnableInteractInput() => allowedInputs.interaction = true;
    public void DisableInteractInput() => allowedInputs.interaction = false;
    #endregion

    #region Input Events
    public void OnInteractEvent(InputAction.CallbackContext context)
    {
        if (!allowedInputs.interaction)
            return;

        if (context.performed)
            _playerManager.InputInteract();
    }
    #endregion
    #endregion

    [Serializable]
    public class AllowedPlayerInputs
    {
        [Serializable]
        public class AllowedDanceInputs
        {
            public bool northDance;
            public bool southDance;
            public bool eastDance;
            public bool westDance;
        }

        public bool movement;
        public bool dance;
        public AllowedDanceInputs danceInputs;
        public bool interaction;

        public AllowedPlayerInputs(bool m, bool d, AllowedDanceInputs di, bool i)
        {
            movement = m;
            dance = d;
            danceInputs = di;
            interaction = i;
        }

        public AllowedPlayerInputs(AllowedPlayerInputs inputs)
        {
            movement = inputs.movement;
            dance = inputs.dance;
            danceInputs = inputs.danceInputs;
            interaction = inputs.interaction;
        }
    }
}