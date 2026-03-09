using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    
    [SerializeField] MusicLevelController musicController;
    [SerializeField] UiAnimator animator;
    
    [SerializeField] SceneChangeController.LoadScenePack levelToLoad;
    [SerializeField] string creditsSceneLoadConfig;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicController.SetPlayMusic();
        animator?.PlaySequence();
    }

    public void PlayGame()
    {
        SceneChangeController.Instance.LoadScenes(levelToLoad);
    }
    
    
    public void LoadCredits()
    {
        SceneManager.LoadScene(creditsSceneLoadConfig);
    }
    

    public void ExitGame()
    {
        StopAllCoroutines();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    
}
