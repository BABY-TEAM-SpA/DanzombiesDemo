using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum FlowState { InFlow, Normal, InDanger };

public class PlayerFlowController : MonoBehaviour
{
    #region [VARIABLES]
    public FlowState State
    {
        get
        {
            float percentage = Mathf.InverseLerp(0, MaxFlow, Flow) * 100f;
            return states.FirstOrDefault(s => percentage >= s.percentage)?.state ?? FlowState.Normal;
        }
    }

    public int Flow => flow;
    [SerializeField] private int flow;

    public int MaxFlow => maxFlow;
    [SerializeField][Min(0)] private int maxFlow;

    [SerializeField] private PlayerFlowState[] states;
    [Serializable]
    private class PlayerFlowState
    {
        public FlowState state;
        [Tooltip("A partir de este porcentaje, el Flow entra a este estado.")]
        [Range(0f, 100f)] public float percentage;
        public UnityEvent OnStateEntered;
        public UnityEvent OnStateExited;
    }

    [SerializeField] private BeatFeedbackFlowModifier[] modifiers;
    [Serializable]
    private class BeatFeedbackFlowModifier
    {
        public BeatReciever.BeatFeedback feedback;
        public int modifier;

        [Tooltip("Si es True, entonces un Modifier igual a 5f representa 5%.")]
        public bool isPercentage;
    }
    #endregion

    #region [UNITY]
    private void Awake()
    {
        states = states.OrderByDescending(s => s.percentage).ToArray();
        GetFlowState(State)?.OnStateEntered?.Invoke();
    }
    #endregion

    #region [METHODS]
    #region API - Flow
    public void SetFlow(int value)
    {
        FlowState prevState = State;

        flow = Mathf.Clamp(value, 0, MaxFlow);
        
        if (prevState != State) // <- Se compara con el getter del State, por eso prevState puede diferir de State
        {
            GetFlowState(prevState)?.OnStateExited?.Invoke();
            GetFlowState(State)?.OnStateEntered?.Invoke();
        }

        DanceBarController.DanceBar?.UpdateFlowBars(Flow);
    }

    public void Increase(int value)
    {
        if (PlayerManager.Player.IsSafe && value < 0)
            value = 0;

        int result = Flow + (GameManager.Alza * value);
        SetFlow(result);
    }
    #endregion

    #region API - Beat Feedback
    public void ApplyFeedback(BeatReciever.BeatFeedback feedback)
        => Increase(GetModifier(feedback));
    #endregion

    #region Helpers
    private PlayerFlowState GetFlowState(FlowState state)
        => states.FirstOrDefault(s => state == s.state);

    private int GetModifier(BeatReciever.BeatFeedback feedback)
    {
        BeatFeedbackFlowModifier modifier = modifiers.FirstOrDefault(m => feedback == m.feedback);
        if (modifier == null)
            return 0;

        return modifier.isPercentage
            ? (int)(MaxFlow * modifier.modifier / 100f)
            : modifier.modifier;
    }
    #endregion
    #endregion
}
