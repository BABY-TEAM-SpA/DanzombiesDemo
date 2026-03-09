using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton_DZSys : Button
{
    public TMP_Text textRender;
    public float textFadeDuration = 0f;

    protected override void OnValidate()
    {
        if(textRender == null) textRender = GetComponent<TMP_Text>();
        if(textRender == null) textRender = GetComponentInChildren<TMP_Text>();
        base.OnValidate();
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        if (!gameObject.activeInHierarchy)
                return;

        Color tintColor;

        switch (state)
        {
            case SelectionState.Normal:
                tintColor = colors.normalColor;
                break;
            case SelectionState.Highlighted:
                tintColor = colors.highlightedColor;
                break;
            case SelectionState.Pressed:
                tintColor = colors.pressedColor;
                
                break;
            case SelectionState.Selected:
                tintColor = colors.selectedColor;
                break;
            case SelectionState.Disabled:
                tintColor = colors.disabledColor;
                break;
            default:
                tintColor = Color.black;
                break;
        }
        TextColorTween(tintColor * textRender.color);
        base.DoStateTransition(state, instant);
    }
    
    void TextColorTween(Color targetColor)
    {
        if (textRender == null)
            return;

        textRender.CrossFadeColor(targetColor, textFadeDuration, true, true);
    }
    
}
