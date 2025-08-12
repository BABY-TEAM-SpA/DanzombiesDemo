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
        } 
    }
    public void LoadScene(string name, bool fadeOut=false)
    {
        SceneManager.LoadScene(name);
    }

    public void LoadScene(string name)
    {
        this.LoadScene(name, false);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
