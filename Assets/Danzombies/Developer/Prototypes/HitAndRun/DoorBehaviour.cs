using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DoorBehaviour : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private InteractableComponent interactable;

    [Header("Settings")]
    [Tooltip("Colocar 0 para impedir que se pueda abrir.")]
    [Range(0, 99)] public int interactionsToOpen = 1;

    [Tooltip("Colocar 0 para impedir que se pueda cerrar.")]
    [Range(0, 99)] public int interactionsToClose = 1;

    private Collider2D obstacle;
    private bool isOpen;
    private int count;
    #endregion

    #region [UNITY]
    private void Start()
    {
        if (interactable == null)
            interactable = GetComponentInParent<InteractableComponent>();

        obstacle = GetComponent<Collider2D>();
    }
    #endregion

    #region [METHODS]
    public void OpenOrClose()
    {
        count++;
        if (isOpen)
        {
            Debug.Log($"Trying to close: {count}");
            if (count == interactionsToClose)
                CloseDoor();
        }
        else
        {
            Debug.Log($"Trying to open: {count}");
            if (count == interactionsToOpen)
                OpenDoor();
        }
    }

    #region Helpers
    private void OpenDoor()
    {
        obstacle?.gameObject.SetActive(false);
        isOpen = true;
        count = 0;

        if (interactionsToClose == 0)
            interactable?.Disable();
    }

    private void CloseDoor()
    {
        obstacle?.gameObject.SetActive(true);
        isOpen = false;
        count = 0;

        if (interactionsToOpen == 0)
            interactable?.Disable();
    }
    #endregion
    #endregion
}
