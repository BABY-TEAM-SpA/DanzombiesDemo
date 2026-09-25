using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class FlowFeedbackController : Service<FlowFeedbackController>
{
    #region [VARIABLES]
    [SerializeField] private FlowFeedbackState[] states;
    [Serializable]
    private class FlowFeedbackState
    {
        public FlowState state;
        public UnityEvent OnEntered;
    }
    #endregion

    #region [METHODS]
    public void Show(FlowState state)
    {
        FlowFeedbackState feedbackState = states.FirstOrDefault(s => s.state == state);
        feedbackState?.OnEntered?.Invoke();
    }

    public void Hide() => Show(FlowState.Normal);
    #endregion
}
