using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieChasingHordeCollisionArea : MonoBehaviour
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
