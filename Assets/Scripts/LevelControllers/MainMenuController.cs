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

    
}
