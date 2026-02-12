using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeController : MonoBehaviour
{
    public static SceneChangeController Instance { get; private set; }

    private void Awake() 
    { 
        if (Instance != null && Instance != this) 
        { 
            Destroy(this.gameObject); 
        } 
        else 
        { 
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        } 
    }

    public void LoadScene(string name,bool ShouldStopMusic=false,bool fade = true)
    {
        if (ShouldStopMusic)AudioManager.Instance.StopSong();
        SceneManager.LoadScene(name);
    }

    public void LoadMenu()
    {
        LoadScene("MainMenu", true, true);
    }
}
