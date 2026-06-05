using UnityEngine;

public class InteractiveFeedback : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Settings")]
    [SerializeField] private Color inactiveColor;
    [SerializeField] private Color activeColor;
    #endregion

    #region [METHODS]
    public void ShowFeedback(bool show)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = show
                ? activeColor
                : inactiveColor;
    }
    #endregion
}
