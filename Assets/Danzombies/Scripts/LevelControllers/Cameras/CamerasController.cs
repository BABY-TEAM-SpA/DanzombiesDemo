using System;
using Unity.Cinemachine;
using UnityEngine;

public class CamerasController : MonoBehaviour
{
    #region [VARIABLES]
    private PlayerTriggeredCamera[] cameras;
    private PlayerTriggeredCamera currentCam;

    public CinemachineCamera CurrentCamera => currentCam?.ActiveCamera;
    public Vector2 CenterOfCamera => currentCam?.GetAverageCameras() ?? Vector2.zero;
    #endregion

    #region [UNITY]
    private void Awake()
        => cameras = GetComponentsInChildren<PlayerTriggeredCamera>();

    private void Start()
    {
        foreach (PlayerTriggeredCamera cam in cameras)
        {
            cam.Prepare(PlayerManager.Player.ConfinePlayerCamera());
            cam.OnCameraActivated += () => currentCam = cam;
        }
    }

    private void LateUpdate() => CameraFrustum.Update();
    #endregion
}
