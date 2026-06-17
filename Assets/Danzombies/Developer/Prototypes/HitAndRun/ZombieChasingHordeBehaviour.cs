using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieChasingHordeBehaviour : MonoBehaviour
{
    #region [VARIABLES]
    private const float DIST_THRESHOLD = 0.1f;

    [SerializeField] private PlayerCollisionDetector playerDetector;

    [Header("References")]
    [SerializeField] private PlayerMovementController player;
    [SerializeField] private Transform[] checkpoints;

    [Header("Settings")]
    [SerializeField] bool chaseAtStart;
    [Tooltip("Activar si se quiere que la horda se desvíe en función de la posición del jugador perpendicularmente a la ruta fija hacia los checkpoints.")]
    [SerializeField] bool followPlayer;
    [SerializeField][Range(1f, 20f)] private float maxDistance;
    [Tooltip("Factor al que la horda se moverá con respecto al Player (0.5f = 50% de la velocidad de Greg).")]
    [SerializeField][Range(0f, 2f)] private float chasingFactor;

    private float currentSpeed;
    private Vector2 currentDirection;
    private Transform currentCheckpoint;
    private Queue<Transform> remainingCheckpoints = new();
    #endregion

    #region [UNITY]
    private void Start()
    {
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

    public void StopChasing()
    {
        currentSpeed = 0f;
        remainingCheckpoints.Clear();

        currentCheckpoint = null;
        currentDirection = Vector2.zero;
    }
    #endregion

    #region Behaviour
    private void Chase()
    {
        // Velocidad en función de cercanía al player
        SetSpeed();

        // Avance por la ruta
        transform.position += (Vector3)(currentDirection * currentSpeed * Time.deltaTime);

        // Reajuste perpendicular hacia el player
        if (followPlayer)
        {
            Vector2 perpendicular = Vector2.Perpendicular(currentDirection);
            float lateralOffset = Vector2.Dot(player.transform.position - transform.position, perpendicular);

            transform.position += (Vector3)(perpendicular * lateralOffset * 2f * Time.deltaTime);
        }

        // Llegada al checkpoint
        Vector2 toCheckpoint = currentCheckpoint.position - transform.position;
        float lateralDistance = Mathf.Abs(Vector2.Dot(toCheckpoint, currentDirection));

        if (lateralDistance < DIST_THRESHOLD)
        {
            remainingCheckpoints.Dequeue();

            if (remainingCheckpoints.Count > 0)
                UpdateCheckpoint();
            else currentCheckpoint = null;
        }
    }

    private void CatchPlayer() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //FindFirstObjectByType<CheckpointsManager>().RecoverToLastCeckpoint();
    #endregion

    #region Helpers
    private void SetSpeed()
    {
        float playerDistance = Vector2.Distance(player.transform.position, transform.position);
        float error = playerDistance - maxDistance;

        float targetSpeed = player.MaxSpeed;

        if (Mathf.Abs(error) > DIST_THRESHOLD)
        {
            float t = Mathf.Clamp01((Mathf.Abs(error) - DIST_THRESHOLD) / maxDistance);

            targetSpeed = error > 0
                ? Mathf.Lerp(player.MaxSpeed, player.MaxSpeed * 10f, t)
                : player.MaxSpeed * chasingFactor;
        }

        currentSpeed = targetSpeed;
    }
    private void UpdateCheckpoint()
    {
        currentCheckpoint = remainingCheckpoints.Peek();
        currentDirection = (currentCheckpoint.position - transform.position).normalized;
    }

    private void SetCheckpoints()
    {
        foreach (Transform checkpoint in checkpoints)
            remainingCheckpoints.Enqueue(checkpoint);
    }
    #endregion
    #endregion
}
