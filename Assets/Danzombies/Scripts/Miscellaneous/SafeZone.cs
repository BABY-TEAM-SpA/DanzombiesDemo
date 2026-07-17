using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class SafeZone : MonoBehaviour
{
    #region [VARIABLES]
    private Collider2D zone;

    public UnityEvent<PlayerManager> OnPlayerEnterZone;
    public UnityEvent<PlayerManager> OnPlayerExitZone;
    #endregion

    #region [UNITY]
    private void Awake()
    {
        zone = GetComponent<Collider2D>();
        zone.isTrigger = true;
    }

    #region Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
        {
            player.SetInSafeZone(true);
            OnPlayerEnterZone?.Invoke(player);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
        {
            player.SetInSafeZone(false);
            OnPlayerExitZone?.Invoke(player);
        }
    }
    #endregion
    #endregion
}
