using System;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private Transform playerSpawn;

    [Tooltip("Método que triggerear cuando el Player entre a este checkpoint.")]
    public UnityEvent OnPlayerEnter;

    public Action<Checkpoint, PlayerManager> OnPlayerEntered;
    #endregion

    #region [UNITY]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerManager>(out PlayerManager player))
        {
            OnPlayerEnter?.Invoke();
            OnPlayerEntered?.Invoke(this, player);
        }
    }
    #endregion

    #region [METHODS]
    public void Respawn(PlayerManager player)
    {
        player.transform.position = playerSpawn.position;
    }
    #endregion
}
