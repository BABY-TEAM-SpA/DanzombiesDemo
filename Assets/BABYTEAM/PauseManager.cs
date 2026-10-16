using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PauseManager : Service<PauseManager>
{
    #region [VARIABLES]
    [SerializeField] private bool canPause = true;
    [SerializeField] private InputActionReference pauseAction;

    public bool IsPaused { get; private set; }

    [Header("Events")]
    public UnityEvent OnPause;
    public UnityEvent OnResume;
    #endregion

    #region [UNITY]
    protected override void Awake()
    {
        base.Awake();
        Resume();
    }

    private void OnEnable() => pauseAction.action.started += PauseInput;
    private void OnDisable() => pauseAction.action.started -= PauseInput;
    #endregion

    #region [METHODS]
    public void Pause()
    {
        Time.timeScale = 0f;

        if (PlayerManager.Player)
            PlayerManager.Player.InputCtrl.DisableAllInputs();

        IsPaused = true;
        OnPause?.Invoke();
    }

    public void Resume()
    {
        Time.timeScale = 1f;

        if (PlayerManager.Player)
            PlayerManager.Player.InputCtrl.RecoverInputs();

        IsPaused = false;
        OnResume?.Invoke();
    }
    #endregion

    #region [EVENTS]
    private void PauseInput(InputAction.CallbackContext cxt)
    {
        if (!canPause)
            return;

        if (IsPaused)
            Resume();
        else Pause();
    }
    #endregion
}
