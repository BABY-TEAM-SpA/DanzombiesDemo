using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DoorBehaviour : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private InteractableComponent interactable;

    [Header("Settings - Open")]
    [Tooltip("Colocar 0 para impedir que se pueda abrir.")]
    [Range(0, 99)] public int interactionsToOpen = 1;
    [Range(0f, 3f)] public float timeToOpen;

    [Header("Settings - Close")]
    [Tooltip("Colocar 0 para impedir que se pueda cerrar.")]
    [Range(0, 99)] public int interactionsToClose = 1;
    [Range(0f, 3f)] public float timeToClose;

    private Collider2D obstacle;
    private bool isOpen;
    private int count;

    private Coroutine doorRoutine;
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
        if (doorRoutine != null)
            return;

        count++;
        if (isOpen)
        {
            Debug.Log($"Trying to close '{name}': {count}");
            if (count == interactionsToClose)
                doorRoutine = StartCoroutine(CloseDoor());
        }
        else
        {
            Debug.Log($"Trying to open '{name}': {count}");
            if (count == interactionsToOpen)
                doorRoutine = StartCoroutine(OpenDoor());
        }
    }
    #endregion

    #region [COROUTINES]
    private IEnumerator OpenDoor()
    {
        if (interactionsToClose == 0)
            interactable?.Disable();

        yield return new WaitForSeconds(timeToOpen);

        obstacle?.gameObject.SetActive(false);
        isOpen = true;
        count = 0;

        doorRoutine = null;
    }

    private IEnumerator CloseDoor()
    {
        if (interactionsToOpen == 0)
            interactable?.Disable();

        yield return new WaitForSeconds(timeToClose);

        obstacle?.gameObject.SetActive(true);
        isOpen = false;
        count = 0;

        doorRoutine = null;
    }
    #endregion
}
