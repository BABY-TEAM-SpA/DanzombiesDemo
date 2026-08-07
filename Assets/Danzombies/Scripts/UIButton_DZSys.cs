using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton_DZSys : Button
{
    public bool selectOnEnable;
    public TMP_Text textRender;
    public float textFadeDuration = 0.2f;
    #if UNITY_EDITOR
    protected override void OnValidate()
    {
        if(textRender == null) TryGetComponent(out textRender);
        if(textRender == null  && transform.childCount>0) textRender = GetComponentInChildren<TMP_Text>();
        base.OnValidate();
    }
    #endif
    protected override void Start()
    {
        base.Start();
        if (selectOnEnable && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
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
        if (textRender != null) TextColorTween(tintColor * textRender.color);
        base.DoStateTransition(state, instant);
    }
    
    void TextColorTween(Color targetColor)
    {
        textRender?.CrossFadeColor(targetColor, textFadeDuration, true, true);
    }
    
}
