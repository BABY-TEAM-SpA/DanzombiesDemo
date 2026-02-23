using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

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
    
    
    [Header("PauseSettings")]
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    private Resolution resolution;
    [SerializeField] private Toggle fullscreenToggle;
    private bool isFullscreen = false;
    private List<Resolution> resolutions = new List<Resolution>();
    private int SelectedResolution = 0;

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
            gameplayMap = inputActions.FindActionMap("Player", true);
            uiMap = inputActions.FindActionMap("UI", true);
            gameplayMap.FindAction("PauseAction", true).started += OnPausePressed;
            uiMap.FindAction("PauseAction", true).started += OnPausePressed;
        }
    }

    private void Start()
    {
        isFullscreen = Screen.fullScreen;
        fullscreenToggle.isOn = isFullscreen;
        List<string> resStrings = new List<string>();
        foreach (Resolution res in Screen.resolutions)
        {
            string newRes = res.width.ToString() + "x" + res.height.ToString();
            if (!resStrings.Contains(newRes))
            {
                resStrings.Add(newRes);
                resolutions.Add(res);
            }
        }
        resolutionsDropdown.AddOptions(resStrings);
        
    }

    private void OnDestroy()
    {
        
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
        //unpauseController.StopSequence();
        //pauseController.PlaySequence();
        OnPauseStart();
        OnPauseEnd();
    }

    public void Unpause()
    {
        //pauseController.StopSequence();
        //unpauseController.PlaySequence();
        OnResumeStart();
        OnResumeEnd();
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
    public void OnResolutionUpdate()
    {
        SelectedResolution = resolutionsDropdown.value;
        Screen.SetResolution(resolutions[SelectedResolution].width, resolutions[SelectedResolution].height, isFullscreen);
    }
    
    public void OnFullScreenUpdate()
    {
        isFullscreen = fullscreenToggle.isOn;
    }
}