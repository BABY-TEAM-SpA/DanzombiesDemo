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
        playerSpawn = transform.GetChild(0).GetComponent<Transform>();
    }

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
    #endregion

    #region [METHODS]
    public void Respawn(PlayerManager player)
    {
        player.transform.position = playerSpawn.position;
    }
    #endregion
}
