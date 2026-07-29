using System;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private bool isRespawn;

    private Transform playerSpawn;

    public UnityEvent OnTriggerBeforeCapture;
    public UnityEvent OnTriggerAfterCapture;

    public Action<Checkpoint, PlayerManager> OnPlayerEntered;
    #endregion

    #region [UNITY]
    private void Awake() => playerSpawn = transform.GetChild(0).GetComponent<Transform>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (collision.TryGetComponent<PlayerManager>(out PlayerManager player))
        {
            OnTriggerBeforeCapture?.Invoke();
            if (isRespawn)
                OnPlayerEntered?.Invoke(this, player);
            OnTriggerAfterCapture?.Invoke();
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
