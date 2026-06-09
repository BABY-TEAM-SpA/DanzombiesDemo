using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DoorBehaviour : MonoBehaviour
{
    #region [VARIABLES]
    [Tooltip("Colocar 0 para impedir interactuar.")]
    [Range(0, 99)] public int interactionsToOpen = 1;

    [Tooltip("Colocar 0 para impedir interactuar.")]
    [Range(0, 99)] public int interactionsToClose = 1;

    private Collider2D obstacle;
    private bool isOpen;
    private int count;
    #endregion

    #region [UNITY]
    private void Start() => obstacle = GetComponent<Collider2D>();
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


    protected void OpenDoor()
    {
        obstacle?.gameObject.SetActive(false);
        isOpen = true;
        count = 0;
    }

    protected void CloseDoor()
    {
        obstacle?.gameObject.SetActive(true);
        isOpen = false;
        count = 0;
    }
    #endregion
    #endregion
}
