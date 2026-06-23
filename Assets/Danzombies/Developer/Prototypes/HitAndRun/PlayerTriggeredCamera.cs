using Unity.Cinemachine;
using UnityEngine;

public class PlayerTriggeredCamera : MonoBehaviour
{
    #region [VARIABLES]
    private const int ACTIVE_PRIORITY = 1;
    private const int INACTIVE_PRIORITY = 0;

    [SerializeField] private CinemachineStateDrivenCamera stateDrivenCamera;
    #endregion

    #region [UNITY]
    private void Start() => stateDrivenCamera.Priority = INACTIVE_PRIORITY;

    #region Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            stateDrivenCamera.Priority = ACTIVE_PRIORITY;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            stateDrivenCamera.Priority = INACTIVE_PRIORITY;
    }
    #endregion
    #endregion
}
