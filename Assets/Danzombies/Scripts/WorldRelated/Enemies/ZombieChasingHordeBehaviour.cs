using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieChasingHordeBehaviour : MonoBehaviour, IResettable
{
    #region [VARIABLES]
    private const float DIST_THRESHOLD = 0.1f;

    [SerializeField] private PlayerCollisionDetector playerDetector;

    [Header("References")]
    [SerializeField] private PlayerMovementController playerMovement;
    [SerializeField] private Transform[] checkpoints;

    [Header("Settings")]
    [SerializeField] bool chaseAtStart;
    [SerializeField][Range(1f, 30f)] private float maxDistance;
    [Tooltip("Factor al que la horda se moverá con respecto al Player (0.5f = 50% de la velocidad de Greg).")]
    [SerializeField][Range(0f, 2f)] private float chasingFactor;
    [Tooltip("Máxima distancia que la horda se desviará perpendicularmente de su riel por seguir al Player.")]
    [SerializeField][Range(0f, 10f)] private float maxLateralDeviation;
    [SerializeField][Range(0f, 2f)] private float lateralFollowSpeed;

    private PlayerManager player;
    private float currentSpeed;
    private float currentOffset;
    private Vector2 railPosition;
    private Vector2 currentDirection;
    private Transform currentCheckpoint;
    private Queue<Transform> remainingCheckpoints = new();
    #endregion

    #region [UNITY]
    private void Start()
    {
        _position = transform.position;
        player = playerMovement.GetComponent<PlayerManager>();

        if (chaseAtStart)
            StartChasing();
    }

    private void Update()
    {
        if (currentCheckpoint != null)
            Chase();
    }

    #region Availability
    private void OnEnable()
    {
        if (playerDetector != null)
            playerDetector.OnPlayerCollided += CatchPlayer;
    }

    private void OnDisable()
    {
        if (playerDetector != null)
            playerDetector.OnPlayerCollided -= CatchPlayer;
    }
    #endregion
    #endregion

    #region [METHODS]
    #region API
    public void StartChasing()
    {
        if (currentCheckpoint != null)
            return;

        SetCheckpoints();
        UpdateCheckpoint();
    }

    public void UpdateMaxDistance(float maxDistance) => this.maxDistance = Mathf.Max(maxDistance, 0f);
    public void UpdateChasingFactor(float chasingFactor) => this.chasingFactor = Mathf.Max(chasingFactor, 0f);
    public void UpdateMaxLateralDeviation(float maxLateralDeviation) => this.maxLateralDeviation = Mathf.Max(maxLateralDeviation, 0f);
    public void UpdateLateralFollowSpeed(float lateralFollowSpeed) => this.lateralFollowSpeed = Mathf.Max(lateralFollowSpeed, 0f);

    public void StopChasing()
    {
        currentSpeed = 0f;
        currentOffset = 0f;
        currentDirection = Vector2.zero;
        currentCheckpoint = null;
        remainingCheckpoints.Clear();
    }
    #endregion

    #region Behaviour
    private void Chase()
    {
        SetSpeed();
        Vector2 forward = currentDirection;
        Vector2 perpendicular = Vector2.Perpendicular(forward);

        // Avance por la ruta
        railPosition += forward * currentSpeed * Time.deltaTime;

        float playerOffset = Vector2.Dot((Vector2)playerMovement.transform.position - railPosition, perpendicular);
        float targetOffset = Mathf.Clamp(playerOffset, -maxLateralDeviation, maxLateralDeviation);
        currentOffset = Mathf.MoveTowards(currentOffset, targetOffset, lateralFollowSpeed * Time.deltaTime);

        transform.position = railPosition + perpendicular * currentOffset;

        // Llegada al checkpoint
        Vector2 toCheckpoint = currentCheckpoint.position - transform.position;
        float forwardDistance = Mathf.Abs(Vector2.Dot(toCheckpoint, currentDirection));

        if (forwardDistance < DIST_THRESHOLD)
        {
            remainingCheckpoints.Dequeue();

            if (remainingCheckpoints.Count > 0)
                UpdateCheckpoint();
            else StopChasing();
        }
    }

    private void CatchPlayer() => player.GameOver();
    //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //FindFirstObjectByType<CheckpointsManager>().RecoverToLastCeckpoint();
    #endregion

    #region IResettable
    bool _isActive;
    private Vector3 _position;

    public void CaptureState()
    {
        _isActive = gameObject.activeSelf;
    }

    public void ResetState()
    {
        StopChasing();

        gameObject.SetActive(_isActive);
        transform.position = _position;
        railPosition = Vector2.zero;

        if (chaseAtStart)
            StartChasing();
    }
    #endregion

    #region Helpers
    /// <summary>
    /// 
    /// </summary>
    private void SetSpeed()
    {
        float playerDistance = Vector2.Distance(playerMovement.transform.position, transform.position);
        float error = playerDistance - maxDistance;

        float targetSpeed = playerMovement.MaxSpeed;

        if (Mathf.Abs(error) > DIST_THRESHOLD)
        {
            float t = Mathf.Clamp01((Mathf.Abs(error) - DIST_THRESHOLD) / maxDistance);

            targetSpeed = error > 0
                ? Mathf.Lerp(playerMovement.MaxSpeed, playerMovement.MaxSpeed * 4f, t)
                : playerMovement.MaxSpeed * chasingFactor;
        }

        currentSpeed = targetSpeed;
    }

    private void UpdateCheckpoint()
    {
        currentCheckpoint = remainingCheckpoints.Peek();
        currentDirection = (currentCheckpoint.position - transform.position).normalized;

        if (railPosition == Vector2.zero)
            railPosition = transform.position;
    }

    private void SetCheckpoints()
    {
        foreach (Transform checkpoint in checkpoints)
            remainingCheckpoints.Enqueue(checkpoint);
    }
    #endregion
    #endregion
}
