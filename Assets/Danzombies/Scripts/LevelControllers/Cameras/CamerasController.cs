using Unity.Cinemachine;
using UnityEngine;

public class CamerasController : MonoBehaviour
{
    #region [VARIABLES]
    private PlayerTriggeredCamera[] cameras;
    private PlayerTriggeredCamera playerTriggeredCamera;

    public CinemachineCamera CurrentCamera => playerTriggeredCamera?.ActiveCamera;
    public float CenterOfCamera => playerTriggeredCamera?.GetAverageCamerasX() ?? 0f;
    #endregion

    #region [UNITY]
    private void Awake()
    {
        cameras = GetComponentsInChildren<PlayerTriggeredCamera>();

        foreach (PlayerTriggeredCamera cam in cameras)
            cam.OnPlayerFollowed += OnCameraFollowed;
    }
    #endregion

    #region [EVENTS]
    private void OnCameraFollowed(PlayerTriggeredCamera cam)
    {
        if (playerTriggeredCamera != null && playerTriggeredCamera != cam)
            playerTriggeredCamera.UnfollowPlayer();

        playerTriggeredCamera = cam;
    }
    #endregion
}
