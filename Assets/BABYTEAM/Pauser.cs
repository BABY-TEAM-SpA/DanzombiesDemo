using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Pauser : Service<Pauser>
{
    #region [VARIABLES]
    [SerializeField] private bool canPause = true;
    [SerializeField] private InputActionReference pauseAction;

    public bool IsPaused { get; private set; }

    public UnityEvent OnPause;
    public UnityEvent OnResume;
    #endregion

    #region [UNITY]
    protected override void Awake()
    {
        enabled = false;

        base.Awake();

        pauseAction.action.started += (InputAction.CallbackContext cxt) => enabled = !enabled;
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
        IsPaused = true;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        IsPaused = false;
    }
    #endregion

    #region [METHODS]
    #endregion
}
