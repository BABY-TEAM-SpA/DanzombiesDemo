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
        UpdateIconFeedback();
    }
    #endregion

    #region [METHODS]
    public void Activate(bool activation)
    {
        UpdateFlowBars(PlayerManager.Player.FlowValue);
        UpdateIconFeedback();
        uiAnimator?.PlaySequence(isActive ? "Open" : "Close");

        isActive = activation;
    }

    #region Updates
    public void UpdateFlowBars(int value)
    {
        int maxSafety = PlayerManager.Player.SafetyLevels.y;

        if (value >= maxSafety)
        {
            //if (!isBarFilled)
            //    OnBarFilled?.Invoke();

            //isBarFilled = true;
        }
        else
        {
            //if (value <= maxSafety * StatePercentatge(DanceBarState.State.Danger))
            //{
            //    if (!isBarLow)
            //        OnBarLow?.Invoke();

            //    isBarLow = true;
            //    state = DanceBarState.State.Danger;
            //}
            //else if (value <= maxSafety * StatePercentatge(DanceBarState.State.Normal))
            //    state = DanceBarState.State.Normal;
            //else if (value < maxSafety * StatePercentatge(DanceBarState.State.Flow))
            //    state = DanceBarState.State.Flow;

            //isBarFilled = false;
        }
        
        foreach (Image barra in flowBars)
        {
            barra.fillAmount = value/10f;
            //barra.color = barReactions[currentReaction];
            //beatBarMaterial.SetFloat("_RainbowEnabled", value==10?1f:0f);
        }
        UpdateIconFeedback();
    }

    public void UpdateIconFeedback()
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
