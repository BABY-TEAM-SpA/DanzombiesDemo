using UnityEngine;

public class ExitController : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private SceneChangeController.LoadScenePack levelToLoad;

    private SceneChangeController.LoadScenePack levelToUnLoad;
    #endregion

    #region [UNITY]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            SceneChangeController.Instance.LoadScenes(levelToLoad);
    }
    #endregion
}
