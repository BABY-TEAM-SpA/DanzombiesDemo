using UnityEngine;

public class LoadingSceneController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneChangeController.Instance.ActivateQueueScenesLoad();
    }
}
