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
    public Vector2 Velocity => baseSpeed * moveDirection;
    
    [Header("Scripted Movement")]
    [SerializeField][Min(0f)] private float scriptedDuration;
    private Coroutine scriptedMovement;
    [SerializeField] private float baseSpeed;
    private Vector3 targetPos;
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
        targetPos = point.position;
        Vector2 direction = point.position - transform.position;
        BeginScriptedMovememnt(direction);
    }

    public void MoveInX(float directionX)
    {
        //Debug.Log("MoveToPoint");
        Vector2 direction = new Vector2(directionX,0);
        targetPos = (transform.position+(Vector3)direction);
        direction.Normalize();
        BeginScriptedMovememnt(direction);
    }
    public void MoveInY(float directionY)
    {
        //Debug.Log("MoveToPoint");
        Vector2 direction = new Vector2(0,directionY);
        targetPos = transform.position+(Vector3)direction;  
        BeginScriptedMovememnt(direction);
    }

    public void BeginScriptedMovememnt(Vector2 direction = default, Action onFinished = null)
    {
        moveDirection = direction.normalized;
        if(scriptedMovement!=null) StopCoroutine(scriptedMovement);
        scriptedMovement = StartCoroutine(MoveToTargetRoutine(onFinished));
    }
    public void BeginScriptedMovememnt() => BeginScriptedMovememnt(Vector2.zero); // [Frco] ?
    public void StopScriptedMovement()
    {
        SetDirection(Vector2.zero);
        HandleMovement();
    }
    #endregion

    #region Helpers
    public void SetSpeed(float newSpeed) => baseSpeed = newSpeed;
    public void SetDirection(Vector2 direction) => moveDirection = direction;
    public void SetRun(bool run) => baseSpeed = walkingSpeed * ((run) ? sprintFactor : 1f);
    #endregion
    #endregion

    public void EnableMovement(bool isON = false)
    {
        isMovementEnabled = isON;
        Debug.Log("ADIVINA.");
    }
    
    #region [COROUTINES]
    private IEnumerator MoveToTargetRoutine(Action onFinished = null)
    {
        float elapsed = 0f;

        while (Vector2.Distance(transform.position, targetPos) > 0.1f)
        {
            if (scriptedDuration > 0f && elapsed > scriptedDuration)
                break;

            float distance = Vector2.Distance(transform.position, targetPos);
            SetRun(distance >= 10f);
            moveDirection = (targetPos - transform.position).normalized;
            elapsed += Time.deltaTime;
            yield return null;
        }

        StopScriptedMovement();
        onFinished?.Invoke();
    }
    #endregion
}