using System;
using UnityEngine;

/// <summary>
/// Responde a los inputs del jugador mediante la cadena: PlayerInputController -> PlayerManager -> PlayerMovementController.
/// El movimiento scripteado fue desacoplado en su propio componente: ScriptedMovementController + ScriptedMovementData.
/// </summary>
public class PlayerMovementController : MonoBehaviour
{
    #region [VARIABLES]
    private const float DEAD_ZONE = 0.05f;
    private const float THRESHOLD = 0.01f;

    [Header("References")]
    [SerializeField] private DanceBrain danceBrain;

    [Header("Movement")]
    [SerializeField, Min(1f)] private float walkingSpeed = 10f;
    [Tooltip("Multiplicador de velocidad al correr.")]
    [SerializeField, Range(1f, 2f)] private float sprintFactor = 1.5f;

    private bool isMovementEnabled = true;
    private Vector2 moveDirection;
    private bool isSprinting;

    public float MaxSpeed => walkingSpeed * sprintFactor;
    public Vector2 Velocity => moveDirection * (isSprinting ? MaxSpeed : walkingSpeed);
    #endregion

    #region [UNITY]
    private void Update()
    {
        if (isMovementEnabled)
            HandleMovement();
    }
    #endregion

    #region [METHODS]
    #region API
    public void EnableMovement(bool enable) => isMovementEnabled = enable;

    public void SetDirection(Vector2 direction) => moveDirection = direction;
    public void SetRun(bool run) => isSprinting = run;
    public void Face(float sign) => danceBrain.SetBodyDirection(sign);
    #endregion

    private void HandleMovement()
    {
        Vector2 velocity = (Velocity.sqrMagnitude > DEAD_ZONE * DEAD_ZONE) ? Velocity : Vector2.zero;
        transform.localPosition += (Vector3)(velocity * Time.deltaTime);

        danceBrain.OnMoving(Velocity / walkingSpeed);
        if (Mathf.Abs(Velocity.x) > THRESHOLD)
            Face(Mathf.Sign(Velocity.x));
    }
    #endregion
}