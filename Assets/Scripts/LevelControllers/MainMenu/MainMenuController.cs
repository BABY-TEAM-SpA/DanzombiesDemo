using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    
    [SerializeField] MusicLevelController musicController;
    [SerializeField] Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicController.PlayLevelSong(0);
        animator.SetTrigger("Animate");
    }

    public void PlayGame()
    {
        SceneChangeController.Instance.LoadScene("TutorialVideo", true,true);
    }
    
    
    public void LoadCredits()
    {
        SceneChangeController.Instance.LoadScene("Credits", true,true);
    }
    

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    
}
