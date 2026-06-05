using UnityEngine;

public abstract class Interactive : MonoBehaviour, IInteractive
{
    #region [VARIABLES]
    [SerializeField] private InteractiveFeedback feedback;
    [SerializeField] private BoxCollider2D boxCollider;

    private PlayerInteractionController player;
    #endregion

    #region [UNITY]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player entered interactive area");
        if (collision.TryGetComponent(out PlayerInteractionController playerInteractionController))
        {
            feedback.ShowFeedback(true);
            player = playerInteractionController;
            player.SetInteractive(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Player exited interactive area");
        if (collision.TryGetComponent(out PlayerInteractionController playerInteractionController))
        {
            feedback.ShowFeedback(false);
            player.ClearInteractive();
            player = null;
        }
    }
    #endregion

    #region [METHODS]
    public abstract void Interact(PlayerManager player);
    #endregion
}
