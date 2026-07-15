using System.Collections.Generic;
using UnityEngine;

public class Position3D : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private SortingLayers whenUse = SortingLayers.OnStart;
    public enum SortingLayers
    {
        OnStart,
        OnUpdate,
        None,
    }

    [SerializeField] private SpriteRenderer[] spriteRenderers;
    #endregion

    #region [UNITY]
    private void OnValidate() => RefreshRenderers();

    private void Start()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
            RefreshRenderers();

        if (whenUse == SortingLayers.OnStart)
            SetLayerOnSprites();
    }

    void LateUpdate()
    {
        if (whenUse == SortingLayers.OnUpdate)
            SetLayerOnSprites();
    }
    #endregion

    #region [METHODS]
    public void RefreshRenderers()
        => spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

    public void SetLayerOnSprites()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
            spriteRenderers[i].sortingOrder =
                Mathf.RoundToInt(-transform.position.y * 100) + i;
    }
    #endregion
}
