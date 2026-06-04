    using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] protected SceneChangeController.LoadScenePack levelsToLoad;
    [SerializeField] protected SceneChangeController.LoadScenePack levelsToUnLoad;

    public void Load()
    {
        SceneChangeController.Instance.LoadScenes(levelsToLoad);
        //SceneChangeController.Instance.UnLoadScenes(levelsToUnLoad);
    }
}
