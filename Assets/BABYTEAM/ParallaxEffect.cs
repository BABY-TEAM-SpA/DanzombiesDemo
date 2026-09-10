using System;
using UnityEngine;

/// <summary>
/// Controlador x escena de todos los SpriteRenderers marcados para parallax, con la posibilidad de repetirlos infinitamente.
/// </summary>
public class ParallaxEffect : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private CamerasController camerasController;

    [SerializeField] private ParallaxTarget[] targets;
    [Serializable]
    private class ParallaxTarget
    {
        public enum ParallaxType
        {
            Horizontal,
            Vertical
        }

        public SpriteRenderer target;
        public bool cameraDriven;
        public ParallaxType type;

        [Tooltip("-1 : Velocidad inversa a la cámara / Izquierda o abajo.\n0 : Sin movimiento.\n1 : Misma velocidad que la cámara / Derecha o arriba.")]
        public Vector2 parallaxFactor;

        [Tooltip("Si está activo, el fondo se repetirá infinitamente.")]
        public bool infinite;
        [Min(1)] public int tileCount = 3;

        [HideInInspector] public Transform[] tiles;
        [HideInInspector] public Vector2 tileSize;
    }

    private Vector2 lastCamPos;
    private bool isParallaxing;
    #endregion

    #region [UNITY EVENTS]
    private void Awake()
    {
        if (targets == null || targets.Length == 0)
            return;

        foreach (ParallaxTarget p in targets)
            if (p.infinite)
                ExpandToInfinite(p);
    }

    private void LateUpdate()
    {
        if (camerasController?.CurrentCamera == null)
            return;

        Vector2 cc = camerasController.CenterOfCamera;
        if (!isParallaxing) // <- 1er frame
        {
            lastCamPos = cc;
            isParallaxing = true;
            return;
        }

        Vector2 delta = cc - lastCamPos;
        lastCamPos = cc;

        foreach (ParallaxTarget p in targets)
            ApplyParallax(p, p.cameraDriven ? delta : Vector2.one);
    }
    #endregion

    #region [METHODS]
    #region Parallax
    private void ApplyParallax(ParallaxTarget p, Vector2 delta)
    {
        if (p.infinite && p.tiles != null)
        {
            foreach (Transform tile in p.tiles)
            {
                Vector3 tilePos = tile.position;
                tilePos += (Vector3)(delta * p.parallaxFactor);
                tile.position = tilePos;
            }

            RecycleTiles(p, lastCamPos);
        }
        else
        {
            Vector3 pos = p.target.transform.position;
            pos += (Vector3)(delta * p.parallaxFactor);
            p.target.transform.position = pos;
        }
    }
    #endregion

    #region Helpers
    private void ExpandToInfinite(ParallaxTarget p)
    {
        // Settings
        SpriteRenderer original = p.target;
        p.tileSize = original.bounds.size;

        int count = p.tileCount % 2 == 0
            ? p.tileCount + 1
            : p.tileCount;
        p.tiles = new Transform[count];
        p.tiles[0] = original.transform;

        // Expansion & Parenting
        for (int i = 1; i < count; i++)
        {
            SpriteRenderer clone = Instantiate(original, original.transform.parent);
            clone.name = $"{original.name}_{i}";
            p.tiles[i] = clone.transform;
        }

        // Reposition
        bool horizontal = p.type == ParallaxTarget.ParallaxType.Horizontal;
        float axisSize = horizontal
            ? p.tileSize.x
            : p.tileSize.y;
        float axisOrigin = horizontal
            ? original.transform.position.x
            : original.transform.position.y;
        float axisStart = axisOrigin - axisSize * (count - 1) / 2f;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = p.tiles[i].position;
            if (horizontal)
                pos.x = axisStart + axisSize * i;
            else
                pos.y = axisStart + axisSize * i;
            p.tiles[i].position = pos;
        }
    }

    private void RecycleTiles(ParallaxTarget p, Vector2 cc)
    {
        bool horizontal = p.type == ParallaxTarget.ParallaxType.Horizontal;
        float axisTileSize = horizontal
            ? p.tileSize.x
            : p.tileSize.y;
        float totalSize = axisTileSize * p.tiles.Length;
        float maxDistance = totalSize * 0.5f;

        foreach (Transform tile in p.tiles)
        {
            float tileAxisPos = horizontal
                ? tile.position.x
                : tile.position.y;
            float camAxisPos = horizontal
                ? cc.x
                : cc.y;
            float distance = camAxisPos - tileAxisPos;

            while (distance > maxDistance)
            {
                Vector3 t = tile.position;
                if (horizontal)
                    t.x += totalSize;
                else t.y += totalSize;
                tile.position = t;
                distance -= totalSize;
            }

            while (distance < -maxDistance)
            {
                Vector3 t = tile.position;
                if (horizontal)
                    t.x -= totalSize;
                else t.y -= totalSize;
                tile.position = t;
                distance += totalSize;
            }
        }
    }
    #endregion
    #endregion
}