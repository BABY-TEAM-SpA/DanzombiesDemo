using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DoorBehaviour : MonoBehaviour, IResettable
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

    private int count;
    private bool isOpen;
    private Collider2D obstacle;

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
                doorRoutine = StartCoroutine(CloseDoor(0f));
        }
        else
        {
            Debug.Log($"Trying to open '{name}': {count}");
            if (count == interactionsToOpen)
                doorRoutine = StartCoroutine(OpenDoor(0f));
        }
    }

    #region IResettable
    private bool _isOpen;

    public void CaptureState()
    {
        _isOpen = isOpen;
    }

    public void ResetState()
    {
        if (doorRoutine != null)
            StopCoroutine(doorRoutine);

        doorRoutine = _isOpen
            ? StartCoroutine(OpenDoor(timeToOpen))
            : StartCoroutine(CloseDoor(timeToClose));
    }
    #endregion
    #endregion

    #region [COROUTINES]
    private IEnumerator OpenDoor(float duration)
    {
        if (interactionsToClose == 0)
            interactable?.Disable();

        yield return new WaitForSeconds(duration);

        count = 0;
        isOpen = true;
        obstacle?.gameObject.SetActive(false);        

        doorRoutine = null;
    }

    private IEnumerator CloseDoor(float duration)
    {
        if (interactionsToOpen == 0)
            interactable?.Disable();

        yield return new WaitForSeconds(duration);

        count = 0;
        isOpen = false;
        obstacle?.gameObject.SetActive(true);

        doorRoutine = null;
    }
    #endregion
}
