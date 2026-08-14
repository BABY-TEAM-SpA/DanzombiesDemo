using System;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    #region [VARIABLES]
    public bool IsRespawn => isRespawn;
    [SerializeField] private bool isRespawn;

    public Vector3 Spawn => playerSpawn.position;
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
            RunTriggersBeforeCapture();
            if (isRespawn)
                OnPlayerEntered?.Invoke(this, player);
            RunTriggersAfterCapture();
        }
    }
    #endregion

    #region [METHODS]
    public void Respawn(PlayerManager player) => player.transform.position = playerSpawn.position;

    #region Helpers
    public void RunTriggersBeforeCapture() => OnTriggerBeforeCapture?.Invoke();
    public void RunTriggersAfterCapture() => OnTriggerAfterCapture?.Invoke();
    public void RunAllTriggers()
    {
        RunTriggersBeforeCapture();
        RunTriggersAfterCapture();
    }
    #endregion
    #endregion
}
