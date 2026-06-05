using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractableComponent : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private InteractableFeedback feedback;

    public bool isInteractable = true;

    public UnityEvent OnInteract;
    #endregion

    #region [UNITY]
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

    #region [METHODS]
    public void Interact() => OnInteract?.Invoke();
    #endregion
}
