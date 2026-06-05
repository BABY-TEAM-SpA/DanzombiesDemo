using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    #region [VARIABLES]
    private Interactive interactive;
    #endregion

    #region [METHODS]
    public void OnInteractEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
            interactive?.Interact(GetComponent<PlayerManager>());
    }

    public void SetInteractive(Interactive interactive) => this.interactive = interactive;
    public void ClearInteractive() => interactive = null;
    #endregion
}
