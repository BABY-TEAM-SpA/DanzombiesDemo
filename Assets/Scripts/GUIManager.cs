using UnityEngine;

public class GUIManager : MonoBehaviour
{
    public static GUIManager Instance { get; private set; }
    
    public GameObject PlayerCanvas;
    public GameObject PauseCanvas;
    public GameObject TransitionCanvas;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {

            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
