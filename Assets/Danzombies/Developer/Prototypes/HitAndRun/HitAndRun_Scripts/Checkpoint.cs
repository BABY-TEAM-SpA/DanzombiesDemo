using System;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private bool isRespawn;

    private Collider2D collider2d;
    private Transform playerSpawn;

    [Tooltip("Método que triggerear cuando el Player entre a este checkpoint.")]
    public UnityEvent OnTriggerBehaviour;

    public Action<Checkpoint, PlayerManager> OnPlayerEntered;
    #endregion

    #region [UNITY]
    private void Awake()
    {
        collider2d = GetComponent<Collider2D>();
        playerSpawn = GetComponentInChildren<Transform>();
    }

    #region Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (collision.TryGetComponent<PlayerManager>(out PlayerManager player))
        {
            if (isRespawn)
                OnPlayerEntered?.Invoke(this, player);
            OnTriggerBehaviour?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        collider2d.enabled = true;
    }
    #endregion
    #endregion

    #region [METHODS]
    public void Respawn(PlayerManager player)
    {
        player.transform.position = playerSpawn.position;
    }
    #endregion
}
