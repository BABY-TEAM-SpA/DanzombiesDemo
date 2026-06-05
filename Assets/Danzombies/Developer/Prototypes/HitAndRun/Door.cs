using UnityEngine;

public class Door : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private GameObject obstacle;

    public bool isOpen = false;
    #endregion

    #region [METHODS]
    public void OpenOrClose()
    {
        if (isOpen)
            CloseDoor();
        else OpenDoor();
    }

    private void OpenDoor()
    {
        obstacle?.SetActive(false);
        isOpen = true;
    }

    private void CloseDoor()
    {
        obstacle?.SetActive(true);
        isOpen = false;
    }
    #endregion
}
