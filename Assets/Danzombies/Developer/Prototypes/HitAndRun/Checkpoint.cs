using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    #region [VARIABLES]
    #endregion

    #region [UNITY]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            enabled = true;
    }
    #endregion

    #region [METHODS]
    #endregion
}
