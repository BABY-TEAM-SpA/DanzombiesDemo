using Unity.Cinemachine;
using UnityEngine;

public class CamerasController : MonoBehaviour
{
    #region [VARIABLES]
    private PlayerTriggeredCamera[] cameras;
    private PlayerTriggeredCamera playerTriggeredCamera;

    public CinemachineCamera CurrentCamera => playerTriggeredCamera?.ActiveCamera;
    public Vector2 CenterOfCamera => playerTriggeredCamera?.GetAverageCameras() ?? Vector2.zero;
    #endregion

    #region [UNITY]
    private void Awake()
    {
        cameras = GetComponentsInChildren<PlayerTriggeredCamera>();

        foreach (PlayerTriggeredCamera cam in cameras)
            cam.OnPlayerFollowed += OnCameraFollowed;
    }

    private void Update() => CameraFrustum.Update();
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
