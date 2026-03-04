using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SortingLayers{ 
    OnStart,
    OnUpdate,
    None,
}

public class Position3d : MonoBehaviour
{
    [SerializeField] private SortingLayers whenUse = SortingLayers.OnStart;
    [SerializeField] private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

    private void OnValidate()
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer == null)
            {
                spriteRenderers.Clear();
                break;
            }
        }
        
        if (spriteRenderers.Count == 0)
        {
            SpriteRenderer thisSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if(thisSpriteRenderer != null) spriteRenderers.Add(thisSpriteRenderer);
            var renderes = GetComponentsInChildren<SpriteRenderer>();
            foreach(var render in renderes) if(render != null) spriteRenderers.Add(render);
            
        }
    }

    private void Start()
    {
        if (spriteRenderers.Count == 0)
        {
            SpriteRenderer thisSpriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            GetComponentsInChildren<SpriteRenderer>();
            spriteRenderers.Add(thisSpriteRenderer);
        }
        if (whenUse == SortingLayers.OnStart)
        {
            SetLayerOnSprites();
        }
    }
    void LateUpdate()
    {
        if (whenUse == SortingLayers.OnUpdate)
        {
            SetLayerOnSprites();
        }
    }
    public void SetLayerOnSprites()
    {
        int layer = 0;
        if (spriteRenderers.Count > 0)
        {
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                renderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100+layer);
                layer++;
            }
        }
    }
    
    
}
