using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class SafeZone : DanceZone
{
   

    #region [UNITY]
   

    #region Trigger
    /*private void OnTriggerEnter2D(Collider2D other)
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
    }*/
    #endregion
    #endregion
}
