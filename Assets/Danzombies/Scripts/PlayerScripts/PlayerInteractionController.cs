using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Interactuable:MonoBehaviour
{
    protected InteReactableComponent interactable;
    public void SetInteractive(InteReactableComponent target) => interactable = target;
    public void ClearInteractive(InteReactableComponent target)
    {
        if(interactable== target) interactable = null;
    }

    public virtual void Interact()
    {
        interactable?.HandleInteraction(transform.tag);
    }
}

public class PlayerInteractionController : Interactuable
{
    #region [VARIABLES]

    [SerializeField] private Collider2D handler;
    [SerializeField] private Transform leftSpot;
    [SerializeField] private Transform rightSpot;
    
    #endregion

    #region [UNITY]

    private void Start() => PlayerManager.Player.OnDirectionChanged += OnDirectionChanged;
    private void OnDestroy() => PlayerManager.Player.OnDirectionChanged -= OnDirectionChanged;
    #endregion

    #region [METHODS]
    public void OnInteractEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
            Interact();
    }
    
    
    #endregion

    #region [EVENTS]
    /// <summary>
    /// Cuando el Player cambia de dirección, se des/activa el CircleCollider (spot) que colisiona
    /// con InteracableComponents. Además, si había un interactable activo, se clerea.
    /// </summary>
    private void OnDirectionChanged(bool isLeft)
    {
         handler.transform.localPosition= isLeft ? leftSpot.localPosition : rightSpot.localPosition;
    }
    #endregion
}
