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
    public void SetDirection(Vector2 direction) => moveDirection = direction;
    [SerializeField] private float walkingSpeed = 10f;

    [Tooltip("Multiplicador de velocidad al sprintear")]
    [SerializeField, Range(1f, 2f)] private float sprintFactor = 1.5f;
    public float MaxSpeed => walkingSpeed * sprintFactor;
    public Vector2 Velocity => currentSpeed * moveDirection;
    
    [Header("Scripted Movement")]
    private Coroutine scriptedMovement;
    private float currentSpeed;
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
        Vector2 direction = (point.position - transform.position);
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
    public void BeginScriptedMovememnt() => BeginScriptedMovememnt(Vector2.zero);
    public void StopScriptedMovement() => moveDirection = (Vector3.zero);

    #endregion

    #region Helpers
    public void SetSpeed(float newSpeed) => currentSpeed = newSpeed;
    
    public void SetRun(bool run) => currentSpeed = walkingSpeed * ((run) ? sprintFactor : 1f);
    #endregion
    #endregion

    public void EnableMovement(bool isON = false) => isMovementEnabled = isON;
    
    #region [COROUTINES]
    private IEnumerator MoveToTargetRoutine(Action onFinished = null)
    {
        while (Vector2.Distance(transform.position, targetPos) >= 0.1f)
        {
            moveDirection = (targetPos - transform.position).normalized;
            yield return null;
        }

        moveDirection = Vector2.zero;
        StopScriptedMovement();
        onFinished?.Invoke();
    }
    #endregion

    
    
}