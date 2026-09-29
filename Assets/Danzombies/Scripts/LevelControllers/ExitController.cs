using UnityEngine;

public class ExitController : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private LoadScenePack levelToLoad;

    private LoadScenePack levelToUnLoad;
    #endregion

    #region [UNITY]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            SceneChangeController.Instance.LoadScenes(levelToLoad);
    }
    #endregion
}
