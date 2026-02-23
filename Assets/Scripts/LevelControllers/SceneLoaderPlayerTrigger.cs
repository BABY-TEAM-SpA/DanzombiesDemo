using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SceneLoaderPlayerTrigger : SceneLoader
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            Load();
        }
    }
}
