using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitController : MonoBehaviour
{
    [SerializeField] private SceneChangeController.LoadScenePack levelToLoad;
    private SceneChangeController.LoadScenePack levelToUnLoad;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            SceneChangeController.Instance.LoadScenes(levelToLoad);
        }
    }
}
