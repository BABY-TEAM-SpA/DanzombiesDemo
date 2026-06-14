using System;
using System.Linq;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private Transform playerSpawn;

    public Action<Checkpoint, PlayerManager> OnPlayerEntered;
    #endregion

    #region [UNITY]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerManager>(out PlayerManager player))
            OnPlayerEntered?.Invoke(this, player);
    }
    #endregion

    #region [METHODS]
    public void Respawn(PlayerManager player)
    {
        player.transform.position = playerSpawn.position;
    }
    #endregion
}
