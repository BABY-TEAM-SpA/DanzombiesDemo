using UnityEngine;

/// <summary>
/// Clase estática para retornar una posición clampeada a los bounds de un Collider2D.
/// </summary>
public static class BoundsContainer
{
    public static Vector3 ClampPosition(Vector3 worldPosition, Collider2D container, Vector2 objectHalfSize = default)
    {
        Bounds bounds = container.bounds;

        float minX = bounds.min.x + objectHalfSize.x;
        float maxX = bounds.max.x - objectHalfSize.x;
        float minY = bounds.min.y + objectHalfSize.y;
        float maxY = bounds.max.y - objectHalfSize.y;

        worldPosition.x = Mathf.Clamp(worldPosition.x, minX, maxX);
        worldPosition.y = Mathf.Clamp(worldPosition.y, minY, maxY);

        return worldPosition;
    }
}