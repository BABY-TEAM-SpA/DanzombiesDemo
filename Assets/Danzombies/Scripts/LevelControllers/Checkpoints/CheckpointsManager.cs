using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointsManager : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private Transform levelRoot;

    private PlayerManager player;
    private Checkpoint lastCheckpoint;
    [SerializeField] private MonoBehaviour[] resettableObjects;
    #endregion

#if UNITY_EDITOR
    /// <summary>
    /// Encuentra todos los objetos IResettable en la escena y los guarda en `resettableObjects`.
    /// CheckpointsManagerEditor llama a este método cuando se presiona el botón Collect Resettables en el inspector.
    /// </summary>
    public void CollectResettables()
    {
        Transform searchRoot = levelRoot != null
            ? levelRoot : transform.root;

        Array.Clear(resettableObjects, 0, resettableObjects.Length);
        resettableObjects = searchRoot.GetComponentsInChildren<MonoBehaviour>(true)
            .Where(mb => mb is IResettable).ToArray();

        Debug.Log($"El array fue actualizado con {resettableObjects.Length} objetos IResettable encontrados en la escena");
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif

    #region [UNITY]
    private void OnEnable()
    {
        foreach (Transform child in transform)
            if (child.TryGetComponent<Checkpoint>(out Checkpoint checkpoint))
                checkpoint.OnPlayerEntered += EnableCheckpoint;
    }

    private void OnDisable()
    {
        if (player != null)
            player.OnPlayerDeath -= RecoverToLastCheckpoint;

        foreach (Transform child in transform)
            if (child.TryGetComponent<Checkpoint>(out Checkpoint checkpoint))
                checkpoint.OnPlayerEntered -= EnableCheckpoint;
    }
    #endregion

    #region [METHODS]
    public void RecoverToLastCheckpoint()
    {
        if (lastCheckpoint == null)
        {
            Debug.LogError($"No se puede respawnear sin un Checkpoint");
            return;
        }

        foreach (IResettable resettable in resettableObjects)
            resettable.ResetState();
        Debug.Log($"Respawneando en {lastCheckpoint.name}, {resettableObjects.Length} objetos restaurados");

        lastCheckpoint.Respawn(player);
    }
    #endregion

    #region [EVENTS]
    private void EnableCheckpoint(Checkpoint checkpoint, PlayerManager playerManager)
    {
        if (player != null)
            player.OnPlayerDeath -= RecoverToLastCheckpoint;

        player = playerManager;
        lastCheckpoint = checkpoint;

        foreach (IResettable resettable in resettableObjects)
            resettable?.CaptureState();
        Debug.Log($"Nuevo Checkpoint: {checkpoint.name}, {resettableObjects.Length} objetos guardado");

        player.OnPlayerDeath += RecoverToLastCheckpoint;
    }
    #endregion
}
