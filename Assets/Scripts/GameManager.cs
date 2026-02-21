using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Languages language;
    [SerializeField] [Range(0f,2f)] public static float Alza = 1f;
    
    public static GameManager Instance { get; private set; }
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
}
