using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DanceBarController : MonoBehaviour
{
    #region [VARIABLES]
    public bool isActive;
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
    public bool isBarFilled { private set; get; } = false;

    public static DanceBarController DanceBar;
    #endregion

    #region [UNITY]
    private void Awake()
    {
        if (DanceBar == null)
            DanceBar = this;
        else Destroy(gameObject);
    }

    public void Start()
    {
        Material newMat = new Material(beatBarMaterial);
        beatBarMaterial = newMat;
        foreach (Image bar in beatBars)
            bar.material = newMat;
        
        //PlayerManager.Player.danceBar = this; // [Frco] Lo cambié para que sea el propio PlayerManager quien busca y asigna la DanceBar
        UpdateFlowBars(0);
    }
    #endregion

    #region [METHODS]
    public void Activate(bool activation)
    {
        UpdateFlowBars(PlayerManager.Player.FlowValue);
        uiAnimator?.PlaySequence(activation ? "Open" : "Close");

        isActive = activation;
    }

    #region Updates
    public void UpdateFlowBars(int value)
    {
        FlowState state = PlayerManager.Player.FlowState;
        int maxSafety = PlayerManager.Player.SafetyLevels.y;
        isBarFilled = value == maxSafety;
        foreach (Image bar in flowBars)
        {
            bar.fillAmount = value / (float)maxSafety;
            
            bar.color = StateColor(state);
            beatBarMaterial.SetFloat("_RainbowEnabled", value == maxSafety ? 1f : 0f);
        }

        UpdateIconFeedback();
    }

    private void UpdateIconFeedback()
    {
        if (iconImage != null)
            iconImage.sprite = isActive
                ? StateIcon(PlayerManager.Player.FlowState)
                : iconDefaultState;

        if (!isActive)
            beatBarMaterial.SetFloat("_RainbowEnabled", 0f);
    }
    #endregion

    #region Helpers
    private DanceBarState GetState(FlowState state) => states.FirstOrDefault(s => s.state == state);

    private Sprite StateIcon(FlowState state) => GetState(state).icon;
    private Color StateColor(FlowState state) => GetState(state).color;
    #endregion
    #endregion
}
