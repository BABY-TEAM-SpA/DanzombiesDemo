using UnityEngine;


public class SceneLoaderPlayerTrigger : SceneLoader
{
    #region [VARIABLES]
    private bool used;
    #endregion

    #region [UNITY]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !used)
        {
            used = true;
            Load();
        }
    }
    #endregion
}
