using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Componente que se acopla en un GameObject para que el Player pueda interactuar con él.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class InteractableComponent : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private InteractableFeedback feedback;

    [Tooltip("Conectar con el método que se ejecutará cuando el Player interactúe con este GameObject.")]
    public UnityEvent OnInteract;
    #endregion

    #region [UNITY]
    private void Awake()
    {
        Collider2D collider = GetComponent<Collider2D>();
        collider.isTrigger = true;
    }

    #region Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enabled && collision.TryGetComponent(out PlayerInteractionController player))
        {
            ShowFeedback(true);
            player?.SetInteractive(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (enabled && collision.TryGetComponent(out PlayerInteractionController player))
        {
            ShowFeedback(false);
            player?.ClearInteractive();
        }
    }
    #endregion
    #endregion

    #region [METHODS]
    public void Interact()
    {
        if (!enabled)
            return;

        feedback?.Pulse();
        OnInteract?.Invoke();
    }

    public void ShowFeedback(bool show)
    {
        if (show)
            feedback?.Show();
        else feedback?.Hide();
    }

    #region Dis/Enable
    public void Enable() => enabled = true;

    public void Disable()
    {
        enabled = false;
        ShowFeedback(false);
    }
    #endregion
    #endregion
}
