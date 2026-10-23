using UnityEngine;
using UnityEngine.Events;

public class ScriptedMovementData : MonoBehaviour
{
    [Tooltip("- Spatial: Intenta llegar al destination a toda costa.\n" +
        "- Temporal: Solo ejecuta el movimiento hacia destination por duration segundos.")]
    public ScriptedInterruptMovementType type;
    public Transform destination;
    public float interruptTimer;
    public ScriptedMovementType movementType;
    public ScriptedMovementFaceOnArrive faceOnArrive;
    [Tooltip("Por defecto el ScriptedMovement ejecuta DisableAllInputs, mantener true para ejecutar EnableAllInputs al detenerse.")]

    [Header("Inputs")]
    public bool enableMoveInputOnStop = true;
    public bool enableDanceInputOnStop = true;
    public bool enableInteractInputOnStop = true;

    public Vector3 Destination => destination.position;
    public float FaceSign => faceOnArrive == ScriptedMovementFaceOnArrive.Left ? -1f : 1f;

    #region Structures

    public enum ScriptedInterruptMovementType { None, Temporal, TemporalBeat}
    public enum ScriptedMovementType {Walking, Running,Auto}
    public enum ScriptedMovementFaceOnArrive { Left, Right }
    #endregion

    public UnityEvent OnStart;
    public UnityEvent OnArrive;
    public UnityEvent OnInterrupt;
}
