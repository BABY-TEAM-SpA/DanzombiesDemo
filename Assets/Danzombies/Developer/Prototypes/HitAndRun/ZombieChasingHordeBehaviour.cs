using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieChasingHordeBehaviour : MonoBehaviour
{
    #region [VARIABLES]
    private const float CHECKPOINT_THRESHOLD = 1f;

    [SerializeField] private ZombieChasingHordeCollisionArea collisionArea;
    [SerializeField] private ZombieChasingHordeDetectionArea detectionArea;

    [Header("References")]
    [Tooltip("Si la horda se queda sin checkpoints, perseguirá el Transform del Player.")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform[] checkpoints;

    [Header("Settings")]
    [SerializeField] [Range(1f, 10f)] private float walkingSpeed = 8f;
    [SerializeField] [Range(1f, 30f)] private float acceleration = 10f;

    private bool playerInSight;
    private Queue<Transform> remainingCheckpoints = new();
    private float currentSpeed;
    private Vector2 velocity;
    #endregion

    #region [UNITY]
    private void Awake()
    {
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (collisionArea != null)
            collisionArea.OnPlayerCollided += Reach;

        if (detectionArea != null)
            detectionArea.OnPlayerDetected += SetPlayerInSight;

        SetSpeed(walkingSpeed);
        SetCheckpoints();
    }

    private void Update() => Chase();
    #endregion

    #region [METHODS]
    private void Chase()
    {
        // Definir target (Player o próximo checkpoint)
        Transform target = playerInSight ? playerTransform
            : remainingCheckpoints.Count > 0
                ? remainingCheckpoints.First() : playerTransform;

        if (target == null)
            return;

        // Mover la horda hacia el target
        Vector2 direction = (target.position - transform.position).normalized;
        velocity = Vector2.Lerp(velocity, direction * currentSpeed, acceleration * Time.deltaTime);
        transform.localPosition += (Vector3)(velocity * Time.deltaTime);

        // Remover el checkpoint actual si es alcanzado con cierto margen
        if (target == playerTransform)
            return;

        float distance = Vector2.Distance(target.position, transform.position);
        if (distance < CHECKPOINT_THRESHOLD || transform.position.x > target.position.x)
            remainingCheckpoints.Dequeue();
    }

    private void Reach() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    #region Helpers
    private void SetPlayerInSight(bool inSight) => playerInSight = inSight;
    private void SetSpeed(float speed) => currentSpeed = speed;

    public void Enable() => enabled = true;
    public void Disable() => enabled = false;

    private void SetCheckpoints()
    {
        foreach (Transform checkpoint in checkpoints)
            remainingCheckpoints.Enqueue(checkpoint);
    }
    #endregion
    #endregion
}
