    using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] protected SceneChangeController.LoadScenePack levelsToLoad;
    [SerializeField] protected SceneChangeController.UnloadScenePack levelsToUnload;
    #endregion

    #region [UNITY]
    private void OnValidate()
    {
        Debug.Assert(
            levelsToLoad.loadMode == LoadSceneMode.Single && levelsToLoad.scenes.Count == 1,
            $"[{name}] Con LoadSceneMode.Single, el LoadScenePack debería contener solo 1 escena."
        );

        Debug.Assert(
            levelsToLoad.loadMode == LoadSceneMode.Single && levelsToUnload.scenes.Count == 0,
            $"[{name}] Con LoadSceneMode.Single, no hace falta declarar escenas para descargar."
        );
    }
    #endregion

    #region [METHODS]
    public void Load()
    {
        Debug.Log($"Loading scene: {levelsToLoad.scenes[0]}");
        SceneChangeController.Instance.LoadScenes(levelsToLoad);
        SceneChangeController.Instance.UnloadScenes(levelsToUnload);
    }
    #endregion
}
