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
    private void Update()
    {
        if (isVisible != CheckVisibility())
        {
            isVisible = !isVisible;

            if (isVisible)
                OnVisible?.Invoke();
            else OnInvisible?.Invoke();
        }
    }
    #endregion

    #region [METHODS]
    private bool CheckVisibility()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        return GeometryUtility.TestPlanesAABB(planes, spriteRenderer.bounds);
    }
    #endregion
}
