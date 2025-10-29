using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private UiAnimatorController pauseController;
    [SerializeField] private UiAnimatorController unpauseController;

    // Lista global de mapas de acción de gameplay y UI
    [SerializeField] private InputActionAsset inputActions;
    private InputActionMap gameplayMap;
    private InputActionMap uiMap;
    private bool isPaused = false;

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
        gameplayMap.FindAction("PauseAction", true).started -= OnPausePressed;
        uiMap.FindAction("PauseAction", true).started -= OnPausePressed;
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (!isPaused)
            Pause();
        else
            Unpause();
    }

    public void Pause()
    {
        unpauseController.StopSequence();
        pauseController.PlaySequence();
    }

    public void Unpause()
    {
        pauseController.StopSequence();
        unpauseController.PlaySequence();
    }

    private void OnPauseStart()
    {
        
        Time.timeScale = 0f;
        gameplayMap.Disable();
    }

    private void OnPauseEnd()
    {
        isPaused = true;
        uiMap.Enable();
    }
    private void OnResumeStart()
    {
        
        uiMap.Disable();
        
    }

    private void OnResumeEnd()
    {
        isPaused = false;
        Time.timeScale = 1f;
        gameplayMap.Enable();
    }
}