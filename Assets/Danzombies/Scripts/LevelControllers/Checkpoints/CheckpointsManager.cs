using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointsManager : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private Transform levelRoot;
    [SerializeField] private CheckpointsCatalog catalog;

    private PlayerManager player;
    private Checkpoint lastCheckpoint;
    [SerializeField] private Resettable[] resettableObjects;
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
        if (player != null)
            player.OnPlayerDeath -= RecoverToLastCheckpoint;

        foreach (Transform child in transform)
            if (child.TryGetComponent<Checkpoint>(out Checkpoint checkpoint))
                checkpoint.OnPlayerEntered -= EnableCheckpoint;
    }
    #endregion

    #region [METHODS]
    #region Recover
    public void RecoverTo(Checkpoint checkpoint, PlayerManager playerManager)
    {
        if (checkpoint == null)
        {
            Debug.LogError($"[CheckpointsManager] No se puede respawnear sin un Checkpoint.");
            return;
        }

        foreach (Resettable resettable in resettableObjects)
            resettable.ResetState(checkpoint);
        Debug.Log($"[CheckpointsManager] Respawneando en {checkpoint.name}, {resettableObjects.Length} objetos restaurados.");

        checkpoint.Respawn(playerManager);
    }

    public void RecoverToLastCheckpoint() => RecoverTo(lastCheckpoint, player);
    #endregion

    #region Helpers
    public bool TryGetCheckpointByName(string id, out Checkpoint respawn)
    {
        Transform child = transform.Find(id);
        if (child != null && child.TryGetComponent<Checkpoint>(out Checkpoint checkpoint) && checkpoint.IsRespawn)
        {
            respawn = checkpoint;
            return true;
        }

        respawn = null;
        return false;
    }
    #endregion

    #region Collect
    /// <summary>
    /// Encuentra todos los objetos IResettable en la escena y los guarda en resettableObjects.
    /// CheckpointsManagerEditor llama a este método cuando se presiona el botón Collect Resettables en el inspector.
    /// </summary>
    public void CollectResettables()
    {
        Transform searchRoot = levelRoot != null
            ? levelRoot : transform.root;

        resettableObjects = searchRoot.GetComponentsInChildren<Resettable>(true);

        Debug.Log($"[CheckpointsManager] El array fue actualizado con {resettableObjects.Length} objetos Resettable encontrados en la escena.");
        UnityEditor.EditorUtility.SetDirty(this);
    }

    public void CollectRespawns()
    {
        if (catalog == null)
        {
            Debug.LogError($"[CheckpointsManager] Falta asignar el CheckpointsCatalog.");
            return;
        }

        Dictionary<int, string> dict = new();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.TryGetComponent<Checkpoint>(out Checkpoint checkpoint) && checkpoint.IsRespawn)
                dict[i] = child.name;
        }

        string[] respawns = dict.Values.ToArray();
        string sceneName = gameObject.scene.name;

        catalog.SetRespawns(sceneName, respawns);

        Debug.Log($"[CheckpointsManager] El catálogo fue actualizado con {respawns.Length} puntos de respawn encontrados en la escena.");
        UnityEditor.EditorUtility.SetDirty(catalog);
    }
    #endregion
    #endregion

    #region [EVENTS]
    private void EnableCheckpoint(Checkpoint checkpoint, PlayerManager playerManager)
    {
        if (player != null)
            player.OnPlayerDeath -= RecoverToLastCheckpoint;

        player = playerManager;
        lastCheckpoint = checkpoint;

        Debug.Log($"[CheckpointsManager] Checkpoint '{checkpoint.name}' cruzado");
        player.OnPlayerDeath += RecoverToLastCheckpoint;
    }
    #endregion
}
