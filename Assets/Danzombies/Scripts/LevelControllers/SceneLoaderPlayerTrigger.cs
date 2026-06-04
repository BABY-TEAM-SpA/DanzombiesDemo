using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SceneLoaderPlayerTrigger : SceneLoader
{
    [SerializeField] bool used=false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player" && !used)
        {
            used = true;
            Load();
        }
    }
}
