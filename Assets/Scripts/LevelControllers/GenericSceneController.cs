using UnityEngine;

public class GenericSceneController : MonoBehaviour
{
    public void OnLoadSceneButton(string sceneName)
    {
        SceneChangeController.Instance.LoadScene(sceneName);
    }
}
