using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    
    public List<UiAnimatorController> pauseControllers = new List<UiAnimatorController>();
    
    public static PauseManager Instance { get; private set; }
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
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

    public void PauseGame()
    {
        Time.timeScale = 0f;
        PauseAnimation();
    }

    public void PauseAnimation()
    {
        foreach (UiAnimatorController controller in pauseControllers)
        {
            controller.PlaySequence();
        }
    }

    public void UnpauseAnimation()
    {
        
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
