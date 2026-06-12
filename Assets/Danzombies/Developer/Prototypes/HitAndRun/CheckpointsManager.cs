using System.Linq;
using UnityEngine;

public class CheckpointsManager : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private MonoBehaviour[] resettableObjects;

    private PlayerManager player;
    private Checkpoint lastCheckpoint;
    private IResettable[] resettables;
    #endregion

    #region [UNITY]
    private void OnEnable()
    {
        foreach (Transform child in transform)
            if (child.TryGetComponent<Checkpoint>(out Checkpoint checkpoint))
                checkpoint.OnPlayerEntered += EnableCheckpoint;
    }

    private void OnDisable()
    {
        foreach (Transform child in transform)
            if (child.TryGetComponent<Checkpoint>(out Checkpoint checkpoint))
                checkpoint.OnPlayerEntered -= EnableCheckpoint;
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (MonoBehaviour monoBehaviour in resettableObjects)
            if (monoBehaviour != null && monoBehaviour is not IResettable)
                Debug.LogWarning($"{monoBehaviour.name} no implementa IResettable.");
    }
    #endif
    #endregion

    #region [METHODS]
    public void RecoverToLastCeckpoint()
    {
        if (lastCheckpoint == null)
        {
            Debug.LogWarning($"No se puede respawnear sin un checkpoint.");
            return;
        }

        ResetWorldState();
        lastCheckpoint.Respawn(player);
    }

    #region Helpers
    private void CaptureWorldState()
    {
        foreach (IResettable resettable in resettables)
            resettable.CaptureInitialState();
        Debug.Log($"Estado de {resettables.Count()} objetos guardado.");
    }

    private void ResetWorldState()
    {
        foreach (IResettable resettable in resettables)
            resettable.ResetState();
        Debug.Log($"Estado de {resettables.Count()} objetos restaurado.");
    }
    #endregion
    #endregion

    #region [EVENTS]
    private void EnableCheckpoint(Checkpoint checkpoint, PlayerManager playerManager)
    {
        Debug.Log($"New last checkpoint: '{checkpoint.name}'.");
        player = playerManager;
        lastCheckpoint = checkpoint;

        CaptureWorldState();
    }
    #endregion
}
