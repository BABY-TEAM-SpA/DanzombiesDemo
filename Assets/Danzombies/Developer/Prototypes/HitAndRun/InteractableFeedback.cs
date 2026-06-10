using UnityEngine;

public class InteractableFeedback : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Inactive")]
    [SerializeField] private Color inactiveColor;
    [SerializeField] private Color activeColor;
    #endregion

    #region [METHODS]
    public void Show() => spriteRenderer.color = activeColor;
    public void Hide() => spriteRenderer.color = inactiveColor;
    #endregion
}
