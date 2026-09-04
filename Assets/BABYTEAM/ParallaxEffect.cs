using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Tilemaps;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private CamerasController camerasController;

    [SerializeField] private ParallaxTarget[] targets;
    [Serializable]
    private class ParallaxTarget
    {
        public SpriteRenderer target;

        [Tooltip("0 : Sin movimiento.\n1 : Misma velocidad que la cámara.")]
        [Range(0f, 1f)] public float parallaxSpeed;

        [Tooltip("Si está activo, el fondo se repetirá infinitamente.")]
        public bool infinite;
        [Min(1)] public int tileCount = 3;

        [HideInInspector] public Transform[] tiles;
        [HideInInspector] public float tileWidth;
    }

    private float lastCamX;
    private bool isParallaxing;
    #endregion

    #region [VARIABLES]
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

        float cc = camerasController.CenterOfCamera;
        if (!isParallaxing) // <- 1er frame
        {
            lastCamX = cc;
            isParallaxing = true;
            return;
        }

        float deltaX = cc - lastCamX;
        lastCamX = cc;

        foreach (ParallaxTarget p in targets)
        {
            if (p.infinite && p.tiles != null)
            {
                foreach (Transform tile in p.tiles)
                {
                    Vector3 tilePos = tile.position;
                    tilePos.x += deltaX * p.parallaxSpeed;
                    tile.position = tilePos;
                }

                RecycleTiles(p, cc);
            }
            else
            {
                Vector3 pos = p.target.transform.position;
                pos.x += deltaX * p.parallaxSpeed;
                p.target.transform.position = pos;
            }
        }
    }
    #endregion

    #region [METHODS]
    private void ExpandToInfinite(ParallaxTarget p)
    {
        // Settings
        SpriteRenderer original = p.target;
        p.tileWidth = original.bounds.size.x;

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
        float startX = original.transform.position.x - (p.tileWidth * (count - 1) / 2f);
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = p.tiles[i].position;
            pos.x = startX + p.tileWidth * i;
            p.tiles[i].position = pos;
        }
    }

    private void RecycleTiles(ParallaxTarget p, float cameraX)
    {
        float totalWidth = p.tileWidth * p.tiles.Length;
        float maxDistance = totalWidth * 0.5f;

        foreach (Transform tile in p.tiles)
        {
            float distance = cameraX - tile.position.x;

            while (distance > maxDistance)
            {
                tile.position += Vector3.right * totalWidth;
                distance -= totalWidth;
            }

            while (distance < -maxDistance)
            {
                tile.position -= Vector3.right * totalWidth;
                distance += totalWidth;
            }
        }
    }
    #endregion
}
