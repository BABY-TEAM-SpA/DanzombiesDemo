using UnityEngine;

public class GUIManager : MonoBehaviour
{
    public static GUIManager Instance { get; private set; }
    
    public GameObject PlayerCanvas;
    public GameObject PauseCanvas;
    public GameObject TransitionCanvas;

    public DanceBarController DanceBar => danceBarController;
    [SerializeField] private DanceBarController danceBarController;

    private void Awake()
    {

        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
