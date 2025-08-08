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

    private void Start()
    {
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
        else if (this.gameObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer render))
        {
            render.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100+layer);
            layer++;
        }
    }
    
    
}
