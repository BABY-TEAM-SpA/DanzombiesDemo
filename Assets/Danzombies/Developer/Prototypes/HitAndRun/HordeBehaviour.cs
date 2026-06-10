using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HordeBehaviour : MonoBehaviour
{
    #region [VARIABLES]
    private const float CHECKPOINT_THRESHOLD = 0.5f;

    [Tooltip("Si la horda se queda sin checkpoints, perseguirá el Transform del Player.")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform[] checkpoints;

    [Header("Settings")]
    [SerializeField] [Range(1f, 10f)] private float walkingSpeed = 8f;
    [SerializeField] [Range(1f, 30f)] private float acceleration = 10f;

    private Queue<Transform> remainingCheckpoints = new();
    private float currentSpeed;
    private Vector2 velocity;
    #endregion

    #region [UNITY]
    private void Awake()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            playerTransform = player.transform;
        }

        SetSpeed(walkingSpeed);
        SetCheckpoints();
    }

    private void Update() => Chase();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            Debug.Log($"Horde colliding w/player");
    }
    #endregion

    #region [METHODS]
    private void Chase()
    {
        // Definir target (Player o próximo checkpoint)
        Transform target = remainingCheckpoints.Count > 0
            ? remainingCheckpoints.First() : playerTransform;
        if (target == null)
            return;

        // Mover la horda hacia el target
        Vector2 direction = (target.position - transform.position).normalized;
        velocity = Vector2.Lerp(velocity, direction * currentSpeed, acceleration * Time.deltaTime);
        transform.localPosition += (Vector3)(velocity * Time.deltaTime);

        // Remover el checkpoint actual si es alcanzado con cierto margen
        float distance = Vector2.Distance(target.position, transform.position);
        if (target != playerTransform && distance < CHECKPOINT_THRESHOLD)
            remainingCheckpoints.Dequeue();
    }

    #region Helpers
    public void SetSpeed(float speed) => currentSpeed = speed;

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
