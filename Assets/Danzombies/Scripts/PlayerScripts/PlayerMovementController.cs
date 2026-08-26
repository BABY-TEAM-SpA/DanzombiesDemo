using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    #region [VARIABLES]
    [Header("References")]
    [SerializeField] private DanceBrain danceBrain;

    [Header("Movement")]
    [SerializeField] private float walkingSpeed = 10f;
    [SerializeField] private float acceleration = 15f;
    [Tooltip("Multiplicador de velocidad al sprintear")]
    [SerializeField][Range(1f, 2f)] private float sprintFactor = 1.5f;

    [Header("Scripted Movement")]
    [SerializeField][Min(0f)] private float scriptedDuration;
    [SerializeField] private Vector2 scriptedDirection;

    public float MaxSpeed => walkingSpeed * sprintFactor;

    public Vector2 Velocity { get; private set; }

    private bool allowInput;
    private bool scriptedMovement;
    private float currentSpeed;
    private Vector2 inputDirection;
    #endregion

    #region [UNITY]
    private void Start() => SetSpeed(walkingSpeed);

    private void Update() => HandleMovement();
    #endregion

    #region [METHODS]
    private void HandleMovement()
    {
        Vector2 targetDirection = scriptedMovement
            ? scriptedDirection : allowInput
                ? inputDirection : Vector2.zero;

        Velocity = Vector2.Lerp(Velocity, targetDirection.normalized * currentSpeed, acceleration * Time.deltaTime);
        if (Velocity.magnitude < 0.05f)
            Velocity = Vector2.zero;

        transform.localPosition += (Vector3)(Velocity * Time.deltaTime);
        danceBrain.OnMoving(Velocity / walkingSpeed);

        if (Mathf.Abs(Velocity.x) > 0.01f)
            danceBrain.SetBodyDirection(Mathf.Sign(Velocity.x));
    }

    #region Scripted Movement
    public void BeginScriptedMovememnt(float duration = 0f, Vector2 direction = default, Action onFinished = null)
    {
        if (duration != 0f)
            SetScriptedDuration(duration);
        if (direction != default)
            SetScriptedDirection(direction);

        scriptedMovement = true;
        StartCoroutine(MoveForSecondsRoutine(onFinished));
    }
    public void BeginScriptedMovememnt()
    {
        scriptedMovement = true;
        StartCoroutine(MoveForSecondsRoutine());
    }

    public void SetScriptedDuration(float duration) => scriptedDuration = Mathf.Max(duration, 0f);
    public void SetScriptedDirection(Vector2 direction) => scriptedDirection = direction.normalized;

    public void StopScriptedMovement() => scriptedMovement = false;
    #endregion

    #region Helpers
    public void SetSpeed(float newSpeed) => currentSpeed = newSpeed;

    public void EnableInput() => allowInput = true;
    public void DisableInput() => allowInput = false;
    #endregion
    #endregion

    #region [COROUTINES]
    private IEnumerator MoveForSecondsRoutine(Action onFinished = null)
    {
        yield return new WaitForSeconds(scriptedDuration);
        StopScriptedMovement();
        onFinished?.Invoke();
    }
    #endregion

    #region [EVENTS]
    public void OnMoveEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
            inputDirection = context.ReadValue<Vector2>();

        if (context.canceled)
            inputDirection = Vector2.zero;
    }

    public void OnSprintEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
            SetSpeed(walkingSpeed * sprintFactor);
        else if (context.canceled)
            SetSpeed(walkingSpeed);
    }
    #endregion
}