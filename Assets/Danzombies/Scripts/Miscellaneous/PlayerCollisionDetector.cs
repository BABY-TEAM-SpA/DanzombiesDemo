using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerCollisionDetector : MonoBehaviour
{
    #region [VARIAIBLES]
    public Action OnPlayerCollided;
    #endregion

    #region [UNITY]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            OnPlayerCollided?.Invoke();
    }
    #endregion
}
