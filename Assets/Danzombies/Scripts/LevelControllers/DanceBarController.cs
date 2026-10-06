using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DanceBarController : Service<DanceBarController>
{
    #region [VARIABLES]
    public bool isActive { private set; get; }
    public bool IsFilled => isFilled;

    [SerializeField] private Sprite iconDefaultState;
    [SerializeField] private DanceBarState[] states;
    [Serializable]
    private class DanceBarState
    {
        public FlowState state;
        public Sprite icon;
        public Color color;
    }

    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private List<Image> flowBars = new List<Image>();
    [SerializeField] private List<Image> beatBars = new List<Image>();
    [SerializeField] private Material beatBarMaterial;
    [SerializeField] private UiAnimator uiAnimator;

    private FlowState currentState = FlowState.Normal;
    private bool isFilled;
    private bool materialReady;
    #endregion

    #region [UNITY]
    private void Start() => EnsureMaterial();
    #endregion

    #region [METHODS]
    public void Activate(bool activation)
    {
        isActive = activation;
        uiAnimator?.PlaySequence(activation ? "Open" : "Close");
        RefreshIcon();
        RefreshRainbow();
    }

    #region Updates
    public void UpdateFlowBars(int value, int maxFlow, FlowState state)
    {
        currentState = state;
        isFilled = maxFlow > 0 && value == maxFlow;
        float fill = maxFlow > 0 ? value / (float)maxFlow : 0f;
        Color color = StateColor(state);

        foreach (Image bar in flowBars)
        {
            bar.fillAmount = fill;
            bar.color = color;
        }

        RefreshIcon();
        RefreshRainbow();
    }
    #endregion

    #region Helpers
    private void RefreshIcon()
    {
        if (iconImage != null)
            iconImage.sprite = isActive ? StateIcon(currentState) : iconDefaultState;
    }

    private void RefreshRainbow()
    {
        EnsureMaterial();
        beatBarMaterial.SetFloat("_RainbowEnabled", isActive && isFilled ? 1f : 0f);
    }

    private DanceBarState GetState(FlowState state) => states.FirstOrDefault(s => s.state == state);
    private Sprite StateIcon(FlowState state) => GetState(state)?.icon ?? iconImage?.sprite ?? default;
    private Color StateColor(FlowState state) => GetState(state)?.color ?? default;

    private void EnsureMaterial()
    {
        if (materialReady) return;
        beatBarMaterial = new Material(beatBarMaterial);
        foreach (Image bar in beatBars)
            bar.material = beatBarMaterial;
        materialReady = true;
    }
    #endregion
    #endregion
}
