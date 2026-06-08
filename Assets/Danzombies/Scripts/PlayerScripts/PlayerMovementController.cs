using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DanceBrain danceBrain;

    [Header("Movement")]
    [SerializeField] private float walkingSpeed = 10f;
    private float currentSpeed;
    [SerializeField] private float acceleration = 15f;

    private Vector2 inputDirection;
    private Vector2 scriptedDirection;
    
    private bool allowInput = true;

    public Vector2 Velocity { get; private set; }
    [Tooltip("Multiplicador de velocidad al sprintear")]
    [SerializeField] [Range(1f, 2f)] private float sprintFactor = 1.5f;

    private void Start()
    {
        SetSpeed(walkingSpeed);
    }

    public void EnableInput()
    {
        allowInput = true;
    }

    public void DisableInput()
    {
        allowInput = false;
        inputDirection = Vector2.zero;
    }

    public void OnMoveEvent(InputAction.CallbackContext context)
    {
        if (!allowInput)
            return;
        inputDirection = context.ReadValue<Vector2>();
    }
    
    public void OnSprintEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
            SetSpeed(walkingSpeed * sprintFactor);
        else if (context.canceled)
            SetSpeed(walkingSpeed);
    }
    
    public void MoveInX(float direction)
    {
        scriptedDirection.x = Mathf.Clamp(direction, -1f, 1f);
    }
    public void MoveInY(float direction)
    {
        scriptedDirection.y = Mathf.Clamp(direction, -1f, 1f);
    }
    public void MoveInVector(Vector2 direction)
    {
        scriptedDirection = direction.normalized;
    }

    public void StopScriptedMovement()
    {
        scriptedDirection = Vector2.zero;
    }

    public void MoveForSeconds(Vector2 direction, float duration)
    {
        StartCoroutine(MoveForSecondsRoutine(direction, duration));
    }

    private IEnumerator MoveForSecondsRoutine(Vector2 direction, float duration)
    {
        scriptedDirection = direction.normalized;
        yield return new WaitForSeconds(duration);
        scriptedDirection = Vector2.zero;
    }

    private void Update()
    {
        HandleMovement();
    }
    
    
    
    private void HandleMovement()
    {
        Vector2 targetDirection = scriptedDirection != Vector2.zero ? scriptedDirection: inputDirection;
        Velocity = Vector2.Lerp( Velocity, targetDirection.normalized * currentSpeed, acceleration * Time.deltaTime);
        if (Velocity.magnitude < 0.05f) Velocity = Vector2.zero;
        transform.localPosition += (Vector3)(Velocity * Time.deltaTime);
        danceBrain.OnMoving(Velocity/walkingSpeed);
        if (Mathf.Abs(Velocity.x) > 0.01f) danceBrain.SetBodyDirection(Mathf.Sign(Velocity.x));
    }

    public void SetSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
    }

}