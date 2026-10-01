using UnityEngine;
using UnityEngine.Events;

public class ScriptedMovementData : MonoBehaviour
{
    [Tooltip("- Spatial: Intenta llegar al destination a toda costa.\n" +
        "- Temporal: Solo ejecuta el movimiento hacia destination por duration segundos.")]
    public ScriptedMovementType type;
    public Transform destination;
    public float duration;
    public ScriptedMovementFaceOnArrive faceOnArrive;
    [Tooltip("Por defecto el ScriptedMovement ejecuta DisableAllInputs, mantener true para ejecutar EnableAllInputs al detenerse.")]
    public bool enableInputsOnStop = true;

    public Vector3 Destination => destination.position;
    public float FaceSign => faceOnArrive == ScriptedMovementFaceOnArrive.Left ? -1f : 1f;

    #region Structures
    public enum ScriptedMovementType { Spatial, Temporal }
    public enum ScriptedMovementFaceOnArrive { Left, Right }
    #endregion

    public UnityEvent OnStart;
    public UnityEvent OnArrive;
    public UnityEvent OnInterrupt;
}
