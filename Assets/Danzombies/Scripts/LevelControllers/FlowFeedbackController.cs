using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class FlowFeedbackController : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private FlowFeedbackState[] states;
    [Serializable]
    private class FlowFeedbackState
    {
        public FlowState state;
        public UnityEvent OnEntered;
    }

    public static FlowFeedbackController FlowFeedback {  get; private set; }
    #endregion

    #region [GODOT]
    private void Awake()
    {
        if (FlowFeedback == null)
            FlowFeedback = this;
        else Destroy(gameObject);
    }
    #endregion

    #region [METHODS]
    public void Show(FlowState state)
    {
        FlowFeedbackState feedbackState = states.FirstOrDefault(s => s.state == state);
        feedbackState?.OnEntered?.Invoke();
    }
    #endregion
}
