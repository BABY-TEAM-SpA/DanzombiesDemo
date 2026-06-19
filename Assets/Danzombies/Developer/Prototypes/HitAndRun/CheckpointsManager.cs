using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointsManager : MonoBehaviour
{
    #region [VARIABLES]
    private PlayerManager player;
    private Checkpoint lastCheckpoint;
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
    #endregion

    #region [METHODS]
    public void RecoverToLastCeckpoint()
    {
        if (lastCheckpoint == null)
        {
            Debug.LogError($"No se puede respawnear sin un checkpoint.");
            return;
        }

        lastCheckpoint.Respawn(player);
    }
    #endregion

    #region [EVENTS]
    private void EnableCheckpoint(Checkpoint checkpoint, PlayerManager playerManager)
    {
        Debug.Log($"New last checkpoint: '{checkpoint.name}'.");
        player = playerManager;
        lastCheckpoint = checkpoint;
    }
    #endregion
}
