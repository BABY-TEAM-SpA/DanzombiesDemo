using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieChasingHordeDetectionArea : MonoBehaviour
{
    #region [VARIAIBLES]
    public Action<bool> OnPlayerDetected; // <- <InSight>
    #endregion

    #region [UNITY]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            OnPlayerDetected?.Invoke(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            OnPlayerDetected?.Invoke(false);
    }
    #endregion
}
