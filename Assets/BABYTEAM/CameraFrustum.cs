using UnityEngine;

public static class CameraFrustum
{
    #region [VARIABLES]
    public const int FRAME_RATE = 6; // <- Cada cuántos frames actualizar los planes de la cámara

    public static Plane[] planes;

    private static int lastFrameUpdated = -1;
    #endregion

    #region [METHODS]
    public static void Update() // <- Como es estática, no hace Update real, hay que invocarlo desde un MonoBehaviour
    {
        if (Time.frameCount - lastFrameUpdated >= FRAME_RATE)
        {
            planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            lastFrameUpdated = Time.frameCount;
        }
    }

    public static bool IsVisible(SpriteRenderer spriteRenderer, Camera camera)
        => GeometryUtility.TestPlanesAABB(planes, spriteRenderer.bounds);
    #endregion
}
