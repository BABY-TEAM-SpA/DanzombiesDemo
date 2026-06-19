using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private MonoBehaviour[] resettableObjects;

    private HashSet<IResettable> resettables = new();

    [Tooltip("Método que triggerear cuando el Player entre a este checkpoint.")]
    public UnityEvent OnTriggerBehaviour;

    public Action<Checkpoint, PlayerManager> OnPlayerEntered;
    #endregion

    #region [UNITY]
    private void Awake()
    {
        foreach (MonoBehaviour mb in resettableObjects)
            if (mb is IResettable resettable)
                resettables.Add(resettable);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerManager>(out PlayerManager player))
        {
            CaptureWorldState();
            OnPlayerEntered?.Invoke(this, player);
            OnTriggerBehaviour?.Invoke();
        }
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
    public void Respawn(PlayerManager player)
    {
        ResetWorldState();
        player.transform.position = playerSpawn.position;
    }

    private void CaptureWorldState()
    {
        foreach (IResettable resettable in resettables)
            resettable.CaptureState();
        Debug.Log($"Estado de {resettables.Count} objetos guardado.");
    }

    private void ResetWorldState()
    {
        foreach (IResettable resettable in resettables)
            resettable.ResetState();
        Debug.Log($"Estado de {resettables.Count} objetos restaurado.");
    }
    #endregion
}
