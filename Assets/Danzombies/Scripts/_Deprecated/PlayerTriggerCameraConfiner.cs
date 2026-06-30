using Unity.Cinemachine;
using UnityEngine;

public class PlayerTriggerCameraCOnfiner : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private CinemachineStateDrivenCamera stateDrivenCamera;
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int inactivePriority;
    #endregion

    #region [UNITY]
    private void Start() => stateDrivenCamera.Priority = inactivePriority;

    #region Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager playerManager))
        {
            Debug.Log($"[O] Switching to '{name}' camera.");
            stateDrivenCamera.Priority = activePriority;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (other.TryGetComponent<PlayerManager>(out PlayerManager playerManager))
            {
                Debug.Log($"[O] Switching to '{name}' camera.");
                stateDrivenCamera.Priority = inactivePriority;
            }
        }
    }
    #endregion
    #endregion
}
