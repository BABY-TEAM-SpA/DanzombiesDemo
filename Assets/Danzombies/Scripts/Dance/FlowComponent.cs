using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum FlowState { InFlow, Normal, InDanger };

public class FlowComponent : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private DanceZone zone;

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

    [Header("Events")]
    public UnityEvent OnFlowFilled;
    public UnityEvent OnFlowEmptied;

    public Action<FlowState> OnStateChanged;
    #endregion

    #region [UNITY]
    private void Awake()
    {
        states = states.OrderByDescending(s => s.percentage).ToArray();
        GetFlowState(State)?.OnStateEntered?.Invoke();
    }

    #region Enable/Disable
    private void OnEnable()
    {
        if (zone != null)
            zone.OnPlayerFeedback += ApplyFeedback;
    }

    private void OnDisable()
    {
        if (zone != null)
            zone.OnPlayerFeedback -= ApplyFeedback;
    }
    #endregion
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
        bool affectsFlow = zone.DamageMode != DamageMode.None;
        Increase(affectsFlow ? GetModifier(bf) : 0);
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
            GetFlowState(prevState)?.OnStateExited?.Invoke();
            GetFlowState(State)?.OnStateEntered?.Invoke();
            OnStateChanged?.Invoke(State);
        }

        if (prevFlow != flow)
        {
            if (flow == 0 && zone?.DamageMode == DamageMode.ModificaFlowYDaña)
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
            value = 0;

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

        FlowState shown = hide ? FlowState.Normal : State;
        if (lastShownState == shown) return;

        lastShownState = shown;
        feedback.Show(shown);
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
