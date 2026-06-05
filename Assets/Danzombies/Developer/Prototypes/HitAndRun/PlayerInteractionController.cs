using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    #region [VARIABLES]
    private InteractableComponent interactable;
    #endregion

    #region [METHODS]
    public void OnInteractEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
            interactable?.Interact();
    }

    public void SetInteractive(InteractableComponent interactive) => this.interactable = interactive;
    public void ClearInteractive() => interactable = null;
    #endregion
}
