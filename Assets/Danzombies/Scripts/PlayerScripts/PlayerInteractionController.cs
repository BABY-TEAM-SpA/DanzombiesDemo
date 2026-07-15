using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private CircleCollider2D leftSpot;
    [SerializeField] private CircleCollider2D rightSpot;

    private InteReactableComponent inteReactable;
    #endregion

    #region [UNITY]
    private void Awake()
    {
        if(leftSpot) leftSpot.enabled = true;
        if(rightSpot) rightSpot.enabled = true;
    }

    private void Start() => PlayerManager.Player.OnDirectionChanged += OnDirectionChanged;
    private void OnDestroy() => PlayerManager.Player.OnDirectionChanged -= OnDirectionChanged;
    #endregion

    #region [METHODS]
    public void OnInteractEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
            inteReactable?.Interact();
    }

    public void SetInteractive(InteReactableComponent interactive) => inteReactable = interactive;
    public void ClearInteractive() => inteReactable = null;

    /// <summary>
    /// Cuando el Player está mirando en la dirección opuesta al InteReactableComponent y gira,
    /// el OnTrigger del componente no se disparará. Este método suple esa carencia.
    /// </summary>
    private void CheckOverlapAfterTurn(CircleCollider2D spot)
    {
        Vector2 worldCenter = spot.transform.TransformPoint(spot.offset);
        Collider2D[] hits = Physics2D.OverlapCircleAll(worldCenter, spot.radius);
        
        foreach (Collider2D hit in hits)
        {
            InteReactableComponent found = hit.GetComponentInParent<InteReactableComponent>();
            if (found != null && found.isActiveAndEnabled)
                if (found.HasSecondaryInteraction())
                {
                    found.feedback?.Show();
                    SetInteractive(found);
                    break;
                }
        }
    }
    #endregion

    #region [EVENTS]
    /// <summary>
    /// Cuando el Player cambia de dirección, se des/activa el CircleCollider (spot) que colisiona
    /// con InteracableComponents. Además, si había un interactable activo, se clerea.
    /// </summary>
    private void OnDirectionChanged(bool isLeft)
    {
        if (inteReactable != null && inteReactable.isActiveAndEnabled)
        {
            inteReactable.feedback?.Hide();
            ClearInteractive();
        }

        leftSpot.enabled = isLeft;
        rightSpot.enabled = !isLeft;

        CircleCollider2D activeSpot = isLeft ? leftSpot : rightSpot;
        CheckOverlapAfterTurn(activeSpot);
    }
    #endregion
}
