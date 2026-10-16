using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TMPColoredButton : Button
{
    #region [VARIABLES]
    private TextMeshProUGUI tmp;
    #endregion

    #region [UNITY]
    protected override void Awake()
    {
        base.Awake();
        tmp = GetComponentInChildren<TextMeshProUGUI>();
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);
        if (tmp == null)
            return;

        tmp.color = state switch
        {
            SelectionState.Normal => colors.normalColor,
            SelectionState.Highlighted => colors.highlightedColor,
            SelectionState.Pressed => colors.pressedColor,
            SelectionState.Disabled => colors.disabledColor,
            _ => colors.normalColor
        };
    }
    #endregion
}
