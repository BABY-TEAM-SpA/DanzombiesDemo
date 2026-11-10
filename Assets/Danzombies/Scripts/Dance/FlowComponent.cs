using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum FlowState { InFlow, Normal, InDanger };

public class FlowComponent : MonoBehaviour
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
    [Header("Settings")]
    [SerializeField] private int flow;

    public int MaxFlow => maxFlow;
    [SerializeField][Min(0)] private int maxFlow;

    public bool IsFilled => flow == maxFlow;

    private bool ignore;
    private FlowState? lastShownState;

    [SerializeField] private FlowComponentState[] states;
    [Serializable]
    private class FlowComponentState
    {
        public FlowState state;
        [Tooltip("A partir de este porcentaje, el Flow entra a este estado.")]
        [Range(0f, 100f)] public float percentage;
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

    [Header("Events")]
    public UnityEvent OnFlowEmptied;
    public UnityEvent<FlowState> OnStateChanged;
    public UnityEvent OnFlowFilled;

    
    #endregion

    #region [UNITY]
    private void Awake()
    {
        states = states.OrderByDescending(s => s.percentage).ToArray();
    }
    
    #endregion

    #region [METHODS]
    #region API - Activation
    // [Frco] <¬ DanceZone (des)activa el FlowComponent mediante los UnityEvents OnPlayerEntered/Exited
    public void Activate()
    {
        Reset();
        DanceBarController.Instance?.Activate(true);
        RefreshUI();
    }

    public void Deactivate()
    {
        DanceBarController.Instance?.Activate(false);
        RefreshUI(true);
    }
    #endregion

    #region API - Beat Feedback
    public void ApplyFeedback(BeatReciever.BeatFeedback bf)
    {
        Increase(GetModifier(bf));
    }
    #endregion

    #region API - Flow
    public void SetFlow(int value)
    {
        FlowState prevState = State;
        int prevFlow = flow;

        flow = Mathf.Clamp(value, 0, MaxFlow);

        if (prevState != State) // <- Se compara con el getter del State, por eso prevState puede diferir de State
        {
            OnStateChanged?.Invoke(State);
        }

        if (prevFlow != flow)
        {
            if (flow == 0)
                OnFlowEmptied?.Invoke();

            if (flow == MaxFlow)
                OnFlowFilled?.Invoke();
        }

        RefreshUI();
    }

    public void Reset() => SetFlow(maxFlow / 2);

    public void Increase(int value)
    {
        if (PlayerManager.Player && PlayerManager.Player.IsSafe)
            value = Mathf.Max(value, 0);

        int result = Flow + (GameManager.Instance.Alza * value);
        SetFlow(result);
    }
    #endregion

    #region Refresh
    private void RefreshUI(bool hide = false)
    {
        DanceBarController.Instance?.UpdateFlowBars(Flow, MaxFlow, State);
        RefreshFeedback(hide);
    }

    private void RefreshFeedback(bool hide = false)
    {
        FlowFeedbackController feedback = FlowFeedbackController.Instance;
        if (feedback == null) return;

        if (hide) feedback.Hide();
        else
        {
            if (lastShownState == State) return;
            lastShownState = State;
            feedback.Show(State);
        }
    }
    #endregion

    #region Helpers
    private FlowComponentState GetFlowState(FlowState state)
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
