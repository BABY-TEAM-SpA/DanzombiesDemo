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
    private float currentSpeed;
    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private float walkingSpeed = 10f;
    [Tooltip("Multiplicador de velocidad al sprintear")]
    [SerializeField, Range(1f, 2f)] private float sprintFactor = 1.5f;
    public float MaxSpeed => walkingSpeed * sprintFactor;
    public Vector2 Velocity => currentSpeed * moveDirection;
    
    [Header("Scripted Movement")]
    [SerializeField][Min(0f)] private float scriptedDuration;
    private bool scriptedMovement;
    private bool isMovementEnabled =true;
    
    #endregion

    #region [UNITY]
    private void Start() => SetSpeed(walkingSpeed);

    private void Update()
    {
        if(isMovementEnabled) HandleMovement();
    }
    #endregion

    #region [METHODS]
    private void HandleMovement()
    {
        Vector2 velocity = (Velocity.magnitude > 0.05f)? Velocity: Vector2.zero;
        transform.localPosition += (Vector3)(velocity * Time.deltaTime);
        danceBrain.OnMoving(Velocity / walkingSpeed);
        if (Mathf.Abs(Velocity.x) > 0.01f) danceBrain.SetBodyDirection(Mathf.Sign(Velocity.x));
    }

    #region Scripted Movement
    public void MoveToPoint(Transform point)
    {
        float dist = Vector3.Distance(point.position, transform.position);
        float duration = dist / walkingSpeed;
        Vector2 direction = point.position - transform.position;
        BeginScriptedMovememnt(duration, direction);
    }

    public void BeginScriptedMovememnt(float duration = 0f, Vector2 direction = default, Action onFinished = null)
    {
        if (duration != 0f) SetScriptedDuration(duration);
        if (direction != default)  SetScriptedDirection(direction);
        scriptedMovement = true;
        StartCoroutine(MoveForSecondsRoutine(onFinished));
    }
    public void BeginScriptedMovememnt()
    {
        scriptedMovement = true;
        StartCoroutine(MoveForSecondsRoutine());
    }

    public void SetScriptedDuration(float duration) => scriptedDuration = Mathf.Max(duration, 0f);
    public void SetScriptedDirection(Vector2 direction) => moveDirection = direction.normalized;

    public void StopScriptedMovement() => scriptedMovement = false;
    #endregion

    #region Helpers
    public void SetSpeed(float newSpeed) => currentSpeed = newSpeed;
    public void SetDirection(Vector2 direction) => moveDirection = direction.normalized;
    public void SetRun(bool run)
    {
        currentSpeed = walkingSpeed * ((run) ? sprintFactor : 1f);
    }
    #endregion
    #endregion

    public void EnableMovement(bool isON = false) => isMovementEnabled = isON;
    
    #region [COROUTINES]
    private IEnumerator MoveForSecondsRoutine(Action onFinished = null)
    {
        yield return new WaitForSeconds(scriptedDuration);
        StopScriptedMovement();
        onFinished?.Invoke();
    }
    #endregion

    
    
}