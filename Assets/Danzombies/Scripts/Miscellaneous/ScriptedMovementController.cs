using UnityEngine;

public class ScriptedMovementController : MonoBehaviour
{
    #region [VARIABLES]
    private const float STOP_DISTANCE = 0.1f;
    private const float RUN_DISTANCE = 10f;

    [Header("References")]
    [SerializeField] private PlayerMovementController movementController;
    [SerializeField] private PlayerInputController inputController;

    public enum ScriptedMovementResult { Arrived, TimedOut, Interrupted }
    public bool IsPlaying => data != null;

    private ScriptedMovementData data;
    private float elapsed;
    #endregion

    #region [UNITY]
    private void Update()
    {
        if (!IsPlaying)
            return;

        Vector2 delta = (Vector2)data.Destination - (Vector2)transform.position;
        float distance = delta.magnitude;

        float step = movementController.Velocity.magnitude * Time.deltaTime;
        if (distance <= Mathf.Max(STOP_DISTANCE, step))
        {
            StopScriptedMovement(ScriptedMovementResult.Arrived);
            return;
        }

        elapsed += Time.deltaTime;
        if (data.type == ScriptedMovementData.ScriptedMovementType.Temporal && elapsed >= data.duration)
        {
            StopScriptedMovement(ScriptedMovementResult.TimedOut);
            return;
        }

        movementController.SetDirection(delta / distance);
        movementController.SetRun(distance >= RUN_DISTANCE);
    }

    private void OnDisable() => InterruptScriptedMovement();
    #endregion

    #region [METHODS]
    #region API
    public void StartScriptedMovement(ScriptedMovementData data)
    {
        if (data.destination == null)
            return;
        if (IsPlaying) InterruptScriptedMovement();

        this.data = data;
        elapsed = 0f;
        inputController?.DisableAllInputs();

        data.OnStart?.Invoke();
    }

    public void StopScriptedMovement(ScriptedMovementResult result)
    {
        if (!IsPlaying) return;

        ScriptedMovementData finished = data;
        data = null;

        movementController.SetDirection(Vector2.zero);
        movementController.SetRun(false);
        if (finished.enableInputsOnStop) inputController?.EnableAllInputs();

        if (result != ScriptedMovementResult.Interrupted)
            movementController.Face(finished.FaceSign);

        if (result == ScriptedMovementResult.Interrupted)
            finished.OnInterrupt?.Invoke();
        else finished.OnArrive?.Invoke();
    }
    public void InterruptScriptedMovement() => StopScriptedMovement(ScriptedMovementResult.Interrupted);
    #endregion
    #endregion
}
