using UnityEngine;
using UnityEngine.Events;
using static Position3D;

/// <summary>
/// Componente para configurar el comportamiento de un objeto en relación a su presencia dentro o fuera de una cámara.
/// </summary>
public class OnCameraDetector : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool isVisible;

    public UnityEvent OnVisible;
    public UnityEvent OnInvisible;
    #endregion

    #region [UNITY]
    private void OnEnable() => CameraFrustum.OnPlanesUpdated += CheckVisibility;
    private void OnDisable() => CameraFrustum.OnPlanesUpdated -= CheckVisibility;
    #endregion

    #region [METHODS]
    private void CheckVisibility()
    {
        bool nowVisible = CameraFrustum.IsVisible(spriteRenderer);
        if (nowVisible == isVisible)
            return;

        isVisible = nowVisible;
        Debug.Log($"[OnCameraDetector] {spriteRenderer} is now visible? {isVisible}.");

        if (isVisible)
            OnVisible?.Invoke();
        else OnInvisible?.Invoke();
    }
    #endregion
}
