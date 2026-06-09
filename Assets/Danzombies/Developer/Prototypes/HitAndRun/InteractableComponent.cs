using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Componente que se acopla en un GameObject para que el Player pueda interactuar con él.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class InteractableComponent : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private InteractableFeedback feedback;

    public bool isInteractable = true;

    public UnityEvent OnInteract;
    #endregion

    #region [UNITY]
    private void Awake()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
    }

    #region Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player entered interactive area");
        if (collision.TryGetComponent(out PlayerInteractionController player))
        {
            feedback.ShowFeedback(true);
            player?.SetInteractive(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Player exited interactive area");
        if (collision.TryGetComponent(out PlayerInteractionController player))
        {
            feedback.ShowFeedback(false);
            player?.ClearInteractive();
        }
    }
    #endregion
    #endregion

    #region [METHODS]
    public void Interact() => OnInteract?.Invoke();
    #endregion
}
