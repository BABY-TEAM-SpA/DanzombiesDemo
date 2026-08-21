using System;
using System.Linq;
using UnityEngine;

public enum FlowState { InFlow, Normal, InDanger };

public class PlayerFlowController : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private PlayerFlowState[] states;
    private class PlayerFlowState
    {
        public FlowState state;
        [Range(0f, 100f)] public float percentage;
    }

    public FlowState State
    {
        get
        {
            float percentage = Mathf.InverseLerp(MinSafety, MaxSafety, Flow) * 100f;

            PlayerFlowState state = states
                .OrderByDescending(s => s.percentage)
                .FirstOrDefault(s => percentage >= s.percentage);
            return state.state;
        }
    }

    public int Flow => Mathf.Clamp(currentFlow, MinSafety, MaxSafety);
    [SerializeField] private int currentFlow;

    public int MinSafety => safetyLevels.x;
    public int MaxSafety => safetyLevels.y;

    [Tooltip("X: Mínimo.\nY: Máximo.")]
    [SerializeField] private Vector2Int safetyLevels;
    #endregion

    #region [UNITY]
    #endregion

    #region [METHODS]
    public void SetFlow(int value) => currentFlow = Mathf.Clamp(value, MinSafety, MaxSafety);
    #endregion
}
