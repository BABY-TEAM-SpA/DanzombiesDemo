using UnityEngine;

public class ZombieChasingHordeBehaviour : MonoBehaviour
{
    #region [VARIABLES]
    private const float DIST_THRESHOLD = 0.1f;
    private const float GAP_CLOSE_FACTOR = 4f;

    [Header("References")]
    [SerializeField] private PlayerMovementController playerMovement;
    [SerializeField] Animator animator;

    [Header("Settings")]
    [SerializeField] bool chaseOnEnable;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [SerializeField][Min(0f)] private float maxDistance;
    [Tooltip("Factor al que la horda se moverá con respecto al Player (0.5f = 50% de la velocidad de Greg).")]
    [SerializeField][Range(0f, 2f)] private float chasingFactor;
    [Tooltip("Máxima distancia que la horda se desviará perpendicularmente de su riel por seguir al Player.")]
    [SerializeField][Min(0f)] private float maxLateralDeviation;
    [SerializeField][Min(0f)] private float lateralFollowSpeed;

    private bool isChasing;
    private float currentSpeed;
    private float currentOffset;

    private float railLength;
    private float railProgress;
    private Vector2 railDirection;
    private Vector2 perpendicular;
    #endregion

    #region [UNITY]
    private void OnEnable()
    {
        if (chaseOnEnable)
            SetChase(true);
    }

    private void Update()
    {
        if (isChasing)
            Chase();
    }
    #endregion

    #region [METHODS]
    #region API
    public void SetChase(bool chase)
    {
        if (isChasing == chase)
            return;

        RecalculateRail();
        if (chase)
        {
            float projected = Vector2.Dot((Vector2)transform.position - (Vector2)startPoint.position, railDirection);
            railProgress = Mathf.Clamp(projected, 0f, railLength);
        }
        isChasing = chase;
    }

    public void TeleportToStartPoint()
    {
        RecalculateRail();
        transform.position = startPoint.position;
    }
    #endregion

    #region Updates
    public void UpdateStartingPoint(Transform point) => startPoint = point;
    public void UpdateEndPoint(Transform point) => endPoint = point;

    public void UpdateMaxDistance(float maxDistance) => this.maxDistance = Mathf.Max(maxDistance, 0f);
    public void UpdateChasingFactor(float chasingFactor) => this.chasingFactor = Mathf.Max(chasingFactor, 0f);
    public void UpdateMaxLateralDeviation(float maxLateralDeviation) => this.maxLateralDeviation = Mathf.Max(maxLateralDeviation, 0f);
    public void UpdateLateralFollowSpeed(float lateralFollowSpeed) => this.lateralFollowSpeed = Mathf.Max(lateralFollowSpeed, 0f);
    #endregion

    #region Behaviour
    private void Chase()
    {
        SetSpeed();
        railProgress = Mathf.Min(railProgress + currentSpeed * Time.deltaTime, railLength);

        Vector2 pointOnRail = (Vector2)startPoint.position + railDirection * railProgress;

        float playerOffset = Vector2.Dot((Vector2)playerMovement.transform.position - pointOnRail, perpendicular);
        float targetOffset = Mathf.Clamp(playerOffset, -maxLateralDeviation, maxLateralDeviation);
        currentOffset = Mathf.MoveTowards(currentOffset, targetOffset, lateralFollowSpeed * Time.deltaTime);

        transform.position = pointOnRail + perpendicular * currentOffset;

        if (railProgress >= railLength - DIST_THRESHOLD)
            SetChase(false);
    }
    #endregion

    #region Helpers
    private void RecalculateRail()
    {
        railDirection = ((Vector2)endPoint.position - (Vector2)startPoint.position).normalized;
        perpendicular = Vector2.Perpendicular(railDirection);
        railLength = Vector2.Distance(startPoint.position, endPoint.position);

        currentOffset = 0f;
        railProgress = 0f;
        currentSpeed = 0f;
    }

    private void SetSpeed()
    {
        float playerDistance = playerMovement.transform.position.x - transform.position.x;
        float error = playerDistance - maxDistance;
        float targetSpeed = playerMovement.MaxSpeed;

        if (Mathf.Abs(error) > DIST_THRESHOLD)
        {
            float t = Mathf.Clamp01((Mathf.Abs(error) - DIST_THRESHOLD) / maxDistance);

            targetSpeed = error > 0
                ? Mathf.Lerp(playerMovement.MaxSpeed, playerMovement.MaxSpeed * GAP_CLOSE_FACTOR, t)    // > MaxDistance
                : playerMovement.MaxSpeed * chasingFactor;                                              // < MaxDistance
        }

        currentSpeed = targetSpeed;
    }
    #endregion
    #endregion
}