using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Events;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private UiAnimatorController pauseController;
    [SerializeField] private UiAnimatorController unpauseController;

    // Lista global de mapas de acción de gameplay y UI
    [SerializeField] private InputActionAsset inputActions;
    private InputActionMap gameplayMap;
    private InputActionMap uiMap;
    private bool isPaused = false;
    public bool canPause = false;
    
    public UnityEvent<bool> PauseEvent;

    public static PauseManager Instance { get; private set; }

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
            
            pauseController.sequence.onSequenceStart.AddListener(OnPauseStart);
            pauseController.sequence.onSequenceComplete.AddListener(OnPauseEnd);
            unpauseController.sequence.onSequenceStart.AddListener(OnResumeStart);
            unpauseController.sequence.onSequenceComplete.AddListener(OnResumeEnd);
            
            gameplayMap = inputActions.FindActionMap("Player", true);
            uiMap = inputActions.FindActionMap("UI", true);
            gameplayMap.FindAction("PauseAction", true).started += OnPausePressed;
            uiMap.FindAction("PauseAction", true).started += OnPausePressed;
        }
    }

    private void OnDestroy()
    {
        pauseController.sequence.onSequenceStart.RemoveAllListeners();
        pauseController.sequence.onSequenceComplete.RemoveAllListeners();
        unpauseController.sequence.onSequenceStart.RemoveAllListeners();
        unpauseController.sequence.onSequenceComplete.RemoveAllListeners();
        PauseEvent.RemoveAllListeners();
        gameplayMap.FindAction("PauseAction", true).started -= OnPausePressed;
        uiMap.FindAction("PauseAction", true).started -= OnPausePressed;
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (!isPaused&& canPause)
            Pause();
        else
            Unpause();
    }

    public void Pause()
    {
        
        Debug.Log("Pause");
        unpauseController.StopSequence();
        pauseController.PlaySequence();
    }

    public void Unpause()
    {
        Debug.Log("Unpause");
        pauseController.StopSequence();
        unpauseController.PlaySequence();
    }

    private void OnPauseStart()
    {
        PauseEvent.Invoke(!isPaused);
        Time.timeScale = 0f;
        gameplayMap.Disable();
    }

    private void OnPauseEnd()
    {
        isPaused = true;
        uiMap.Enable();
    }

    private void OnResumeEnd()
    {
        PauseEvent.Invoke(!isPaused);
        gameplayMap.Enable();
        isPaused = false;
        Time.timeScale = 1f;
    }

    private void OnResumeStart()
    {
        uiMap.Disable();
    }
}