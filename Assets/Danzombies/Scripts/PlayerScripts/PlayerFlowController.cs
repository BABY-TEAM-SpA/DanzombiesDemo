using System;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using UnityEngine;

public enum FlowState { InFlow, Normal, InDanger };

public class PlayerFlowController : MonoBehaviour
{
    #region [VARIABLES]
    public FlowState State
    {
        get
        {
            float percentage = Mathf.InverseLerp(MinSafety, MaxSafety, Flow) * 100f;
            return states.FirstOrDefault(s => percentage >= s.percentage)?.state ?? FlowState.Normal;
        }
    }

    public int Flow => Mathf.Clamp(currentFlow, MinSafety, MaxSafety);
    [SerializeField] private int currentFlow;

    public int MinSafety => safetyLevels.x;
    public int MaxSafety => safetyLevels.y;

    [Tooltip("X: Mínimo.\nY: Máximo.")]
    [SerializeField][Min(0)] private Vector2Int safetyLevels;

    [SerializeField] private PlayerFlowState[] states;
    [Serializable]
    private class PlayerFlowState
    {
        public FlowState state;
        [Tooltip("A partir de este porcentaje, el Flow entra a este estado.")]
        [Range(0f, 100f)] public float percentage;
    }
    #endregion

    #region [UNITY]
    private void Start() => states = states.OrderByDescending(s => s.percentage).ToArray();
    #endregion

    #region [METHODS]
    public void SetFlow(int value) => currentFlow = Mathf.Clamp(value, MinSafety, MaxSafety);
    #endregion
}
