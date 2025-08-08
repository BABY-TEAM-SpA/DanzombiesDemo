using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitController : MonoBehaviour
{
    [SerializeField] private string levelToLoad;
    [SerializeField] private bool shouldStopMusic;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            if(shouldStopMusic) BeatManager.Instance.StopSong();
            SceneManager.LoadScene(levelToLoad);
        }
    }
}
