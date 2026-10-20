using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FlowFeedbackController : Service<FlowFeedbackController>
{
    #region [VARIABLES]
    [SerializeField] private Image frameImg;
    [SerializeField] private Image zombiesImg;

    [SerializeField] private FlowFeedbackState[] states;
    [Serializable]
    private class FlowFeedbackState
    {
        public FlowState state;
        public Sprite zombiesHud;
        public Color frameColor;
    }

    public UnityEvent OnShow;
    public UnityEvent OnHide;
    #endregion

    #region [METHODS]
    public void Show(FlowState state)
    {
        FlowFeedbackState feedbackState = states.FirstOrDefault(s => s.state == state);
        if (feedbackState == null) return;

        frameImg.color = feedbackState.frameColor;
        zombiesImg.sprite = feedbackState.zombiesHud;

        OnShow?.Invoke();
    }

    public void Hide() => OnHide?.Invoke();
    #endregion
}
