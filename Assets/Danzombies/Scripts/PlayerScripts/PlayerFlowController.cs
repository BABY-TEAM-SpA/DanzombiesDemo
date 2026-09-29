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

    public bool IsFilled => maxFlow > 0 && flow == maxFlow;
    public bool IsActive => zone != null;
    public bool IsSafe => isSafe;

    private DanceZone zone;
    private bool isSafe;
    private FlowState? lastShownState;

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

    private void Start() => RefreshUI();
    #endregion

    #region [METHODS]
    #region API
    public void BindZone(DanceZone newZone)
    {
        bool wasActive = IsActive;
        zone = newZone;
        if (!wasActive)
            Activate();
    }

    public void UnbindZone(DanceZone oldZone)
    {
        if (zone != oldZone)
            return;
        zone = null;
        Deactivate();
    }

    public void SetSafe(bool value) => isSafe = value;
    #endregion

    #region Activation
    private void Activate()
    {
        DanceBarController.Instance?.Activate(true);
        RefreshUI();
    }

    private void Deactivate()
    {
        DanceBarController.Instance?.Activate(false);
        RefreshUI();
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
            if (flow == 0 && zone != null && zone.GetDamageMode() == DamageMode.ModificaFlowYDaña)
                OnFlowEmptied?.Invoke();

            if (flow == MaxFlow)
                OnFlowFilled?.Invoke();
        }

        RefreshUI();
    }

    public void ResetFlow() => SetFlow(maxFlow / 2);

    public void Increase(int value)
    {
        if (isSafe && value < 0)
            value = 0;

        int result = Flow + (GameManager.Instance.Alza * value);
        SetFlow(result);
    }
    #endregion

    #region API - Beat Feedback
    public void ApplyFeedback(BeatReciever.BeatFeedback feedback)
    {
        bool affectsFlow = zone != null && zone.GetDamageMode() != DamageMode.None;
        Increase(affectsFlow ? GetModifier(feedback) : 0);
    }
    #endregion

    #region Refresh
    private void RefreshUI()
    {
        DanceBarController.Instance?.UpdateFlowBars(Flow, MaxFlow, State);
        RefreshFeedback();
    }

    private void RefreshFeedback()
    {
        FlowFeedbackController feedback = FlowFeedbackController.Instance;
        if (feedback == null) return;
        
        FlowState shown = IsActive ? State : FlowState.Normal;
        if (lastShownState == shown) return;
        
        lastShownState = shown;
        feedback.Show(shown);
    }
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
