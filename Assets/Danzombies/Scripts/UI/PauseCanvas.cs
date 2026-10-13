using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseCanvas : MonoBehaviour
{
    #region [VARIABLES]
    [Header("Animator")]
    [SerializeField] private UiAnimator pauseAnimator;

    public UnityEvent<bool> PauseEvent;

    [Header("Pause Settings")]
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private List<Resolution> resolutions = new();
    private bool isFullscreen;
    #endregion

    #region [UNITY]
    private void Start()
    {
        SetupResolutions();
        SetupFullscreen();
    }
    #endregion

    #region [METHODS]
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
    #endregion
}