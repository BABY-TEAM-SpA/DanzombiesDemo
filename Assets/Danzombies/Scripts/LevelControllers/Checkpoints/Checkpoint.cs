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

    public UnityEvent OnCheckpoint;

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
            if (isRespawn)
                OnPlayerEntered?.Invoke(this, player);

            Run();
        }
    }
    #endregion

    #region [METHODS]
    public void Respawn(PlayerManager player) => player.transform.position = playerSpawn.position;

    #region Helpers
    public void Run() => OnCheckpoint?.Invoke();
    #endregion
    #endregion
}
