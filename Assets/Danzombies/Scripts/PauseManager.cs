using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("Animator")]
    [SerializeField] private UiAnimator pauseAnimator;

    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    private InputActionMap gameplayMap;
    private InputActionMap uiMap;
    private InputAction pauseAction;

    private bool isPaused;
    public bool canPause = true;

    public UnityEvent<bool> PauseEvent;

    [Header("Pause Settings")]
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private List<Resolution> resolutions = new();
    private bool isFullscreen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if (inputActions == null)
            return;

        gameplayMap = inputActions.FindActionMap("Player", true);
        uiMap = inputActions.FindActionMap("UI", true);

        pauseAction = gameplayMap.FindAction("PauseAction", true);

        pauseAction.started += OnPausePressed;
    }

    private void OnDisable()
    {
        if (pauseAction != null)
            pauseAction.started -= OnPausePressed;
    }

    private void Start()
    {
        SetupResolutions();
        SetupFullscreen();
    }

    private void OnPausePressed(InputAction.CallbackContext ctx)
    {
        if (!canPause) return;

        if (!isPaused)
            Pause();
        else
            Resume();
    }

    public void Pause()
    {
        if (isPaused) return;

        isPaused = true;

        PauseEvent?.Invoke(true);

        Time.timeScale = 0f;

        gameplayMap.Disable();
        uiMap.Enable();

        pauseAnimator?.PlaySequence("Pause");
    }

    public void Resume()
    {
        if (!isPaused) return;

        isPaused = false;

        PauseEvent?.Invoke(false);

        Time.timeScale = 1f;

        uiMap.Disable();
        gameplayMap.Enable();

        pauseAnimator?.PlaySequence("Resume");
    }

    private void SetupFullscreen()
    {
        isFullscreen = Screen.fullScreen;
        fullscreenToggle.isOn = isFullscreen;
    }

    private void SetupResolutions()
    {
        resolutions.Clear();
        resolutionsDropdown.ClearOptions();

        var options = new List<string>();
        var unique = new HashSet<string>();

        foreach (var res in Screen.resolutions)
        {
            string key = $"{res.width}x{res.height}";

            if (unique.Add(key))
            {
                resolutions.Add(res);
                options.Add(key);
            }
        }

        resolutionsDropdown.AddOptions(options);
    }

    public void OnResolutionUpdate()
    {
        int index = resolutionsDropdown.value;
        var res = resolutions[index];

        Screen.SetResolution(res.width, res.height, isFullscreen);
    }

    public void OnFullScreenUpdate()
    {
        isFullscreen = fullscreenToggle.isOn;
        Screen.fullScreen = isFullscreen;
    }
}