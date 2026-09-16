using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovementController : MonoBehaviour
{
    #region [VARIABLES]
    [Header("References")]
    [SerializeField] private DanceBrain danceBrain;

    [Header("Movement")]
    [SerializeField] private bool isMovementEnabled = true;
    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private float walkingSpeed = 10f;

    [Tooltip("Multiplicador de velocidad al sprintear")]
    [SerializeField, Range(1f, 2f)] private float sprintFactor = 1.5f;
    public float MaxSpeed => walkingSpeed * sprintFactor;
    public Vector2 Velocity => currentSpeed * moveDirection;
    
    [Header("Scripted Movement")]
    [SerializeField][Min(0f)] private float scriptedDuration;

    private float currentSpeed;
    private Transform target; 
    #endregion

    #region [UNITY]
    private void Start() => SetSpeed(walkingSpeed);

    private void Update()
    {
        if (isMovementEnabled) HandleMovement();
    }
    #endregion

    #region [METHODS]
    private void HandleMovement()
    {
        Vector2 velocity = (Velocity.magnitude > 0.05f) ? Velocity : Vector2.zero;
        transform.localPosition += (Vector3)(velocity * Time.deltaTime);
        danceBrain.OnMoving(Velocity / walkingSpeed);
        if (Mathf.Abs(Velocity.x) > 0.01f)
            danceBrain.SetBodyDirection(Mathf.Sign(Velocity.x));
    }

    #region Scripted Movement
    public void MoveToPoint(Transform point)
    {
        //Debug.Log("MoveToPoint");
        target = point;
        Vector2 direction = (point.position - transform.position);
        BeginScriptedMovememnt(direction);
    }

    public void BeginScriptedMovememnt(Vector2 direction = default, Action onFinished = null)
    {
        SetDirection(direction);
        StartCoroutine(MoveToTargetRoutine(onFinished));
    }
    public void BeginScriptedMovememnt() => StartCoroutine(MoveToTargetRoutine());
    public void StopScriptedMovement() => SetDirection(Vector2.zero);

    #endregion

    #region Helpers
    public void SetSpeed(float newSpeed) => currentSpeed = newSpeed;
    public void SetDirection(Vector2 direction) => moveDirection = direction.normalized;
    public void SetRun(bool run) => currentSpeed = walkingSpeed * ((run) ? sprintFactor : 1f);
    #endregion
    #endregion

    public void EnableMovement(bool isON = false) => isMovementEnabled = isON;
    
    #region [COROUTINES]
    private IEnumerator MoveToTargetRoutine(Action onFinished = null)
    {
        while (Vector2.Distance(transform.position, target.position) >= 0.01f)
        {
            Vector2 direction = target.position - transform.position;
            SetDirection(direction);
            yield return null;
        }

        StopScriptedMovement();
        onFinished?.Invoke();
    }
    #endregion

    
    
}