using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimatorController : DanceAnimatorController
{
    
    
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] SpriteRenderer outlineRenderer;

    private void Update()
    {
        if (outlineRenderer == null) return;
        if (outlineRenderer.sprite == renderer.sprite) return;
        outlineRenderer.sprite = renderer.sprite;
    }
    
    
    
}
