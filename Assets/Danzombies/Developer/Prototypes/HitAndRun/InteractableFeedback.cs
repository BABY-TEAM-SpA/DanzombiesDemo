using UnityEngine;

public class InteractableFeedback : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Inactive")]
    [SerializeField] [Range(1f, 2f)] private float inactiveFeedbackScaling = 1.2f;
    [SerializeField] private Color inactiveColor;


    [SerializeField] private Color activeColor;
    [SerializeField] [Range(1f, 2f)] private float activeFeedbackScaling = 1.2f;
    #endregion

    #region [METHODS]
    public void ShowFeedback(bool show)
    {
        if (show)
        {
            transform.localScale = Vector3.one * activeFeedbackScaling;
            spriteRenderer.color = activeColor;
        }
        else
        {
            transform.localScale = Vector3.one * inactiveFeedbackScaling;
            spriteRenderer.color = inactiveColor;
        }
    }
    #endregion
}
