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

    public bool oneShot = true;

    public bool Triggered => entered && exited;
    private bool entered;
    private bool exited;

    public UnityEvent OnCheckpoint;
    public UnityEvent OnEnterCheckpoint;
    public UnityEvent OnLeaveCheckpoint;

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
            if (oneShot && entered)
                return;
            entered = true;
            OnEnterCheckpoint?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (oneShot && exited)
            return;
        exited = true;
        OnLeaveCheckpoint?.Invoke();
    }
    #endregion

    #region [METHODS]
    public void Respawn(PlayerManager player)
    {
        player.Reset();
        player.transform.position = playerSpawn.position;
    }

    public void Run()
    {
        OnEnterCheckpoint?.Invoke();
        OnLeaveCheckpoint?.Invoke();
    }

    public void Reset()
    {
        entered = false;
        exited = false;
    }
    #endregion
}
