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

    public float MaxSpeed => walkingSpeed * sprintFactor;

    public Vector2 Velocity { get; private set; }

    private bool allowInput = true;
    private float currentSpeed;
    private Vector2 inputDirection;
    private Vector2 scriptedDirection;
    #endregion

    #region [UNITY]
    private void Start()
    {
        SetSpeed(walkingSpeed);
    }

    private void Update()
    {
        HandleMovement();
    }
    #endregion

    #region [METHODS]
    private void HandleMovement()
    {
        Vector2 targetDirection = scriptedDirection != Vector2.zero
            ? scriptedDirection : allowInput
                ? inputDirection : Vector2.zero;

        Velocity = Vector2.Lerp(Velocity, targetDirection.normalized * currentSpeed, acceleration * Time.deltaTime);
        if (Velocity.magnitude < 0.05f)
            Velocity = Vector2.zero;

        //if (name == "StephTutorial")
        //    Debug.Log($"[{name}] {Velocity} = {targetDirection.normalized} * {currentSpeed}");
        transform.localPosition += (Vector3)(Velocity * Time.deltaTime);
        danceBrain.OnMoving(Velocity / walkingSpeed);

        if (Mathf.Abs(Velocity.x) > 0.01f)
            danceBrain.SetBodyDirection(Mathf.Sign(Velocity.x));
    }

    #region Scripted Movement
    public void MoveForSeconds(Vector2 direction, float duration)
        => StartCoroutine(MoveForSecondsRoutine(direction, duration));
    public void MoveForSeconds(float duration)
        => StartCoroutine(MoveForSecondsRoutine(scriptedDirection, duration));

    public void MoveInX(float direction) => scriptedDirection.x = Mathf.Clamp(direction, -1f, 1f);
    public void MoveInY(float direction) => scriptedDirection.y = Mathf.Clamp(direction, -1f, 1f);
    public void MoveInVector(Vector2 direction) => scriptedDirection = direction.normalized;

    public void StopScriptedMovement() => scriptedDirection = Vector2.zero;
    #endregion

    #region Helpers
    public void SetSpeed(float newSpeed) => currentSpeed = newSpeed;

    public void EnableInput() => allowInput = true;
    public void DisableInput() => allowInput = false;
    #endregion
    #endregion

    #region [COROUTINES]
    private IEnumerator MoveForSecondsRoutine(Vector2 direction, float duration)
    {
        scriptedDirection = direction.normalized;
        yield return new WaitForSeconds(duration);
        scriptedDirection = Vector2.zero;
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