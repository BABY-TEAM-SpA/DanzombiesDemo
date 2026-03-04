using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DanceBarController : MonoBehaviour
{
    [Header("Barras de Flow")] 
    [SerializeField] private Image IconImage;
    private int currentReaction = 0;
    [SerializeField] private Sprite IconDefaultState;
    [SerializeField] private Sprite[] IconStates = new Sprite[] {};
    [SerializeField] private List<Image> FlowBars = new List<Image>();
    [SerializeField] private Color[] barReactions = new Color[] {};
    [SerializeField] private List<Image> beatBars = new List<Image>();
    [SerializeField] private Material beatBarMaterial;
    [SerializeField] private UiAnimator uiAnimator;

    public void Start()
    {
        Material newMat = new Material(beatBarMaterial);
        beatBarMaterial = newMat;
        foreach (Image bar in beatBars)
        {
            bar.material = newMat;
        }
        PlayerManager.Player.danceBar = this;
        UpdateFlowBars(PlayerManager.Player.GetFlowDamage(0));
    }

    public void Activate(bool isActive)
    {
        uiAnimator.PlaySequence(isActive ? "Open" : "Close");
    }

    public void TurnRainbowOnOff(bool isActive)
    {
        beatBarMaterial.SetFloat("_RainbowEnabled", isActive?1f:0f);
    }
    
    public void UpdateFlowBars(float value, bool isInside =false)
    {
        if (value <= 2f) currentReaction = 2;
        else if (value <= 5f) currentReaction = 1;
        
        foreach (Image barra in FlowBars)
        {
            barra.fillAmount = value/10;
            barra.color = barReactions[currentReaction];
        }
        UpdateIconFeedback(isInside);
    }
    public void UpdateIconFeedback(bool inDanger)
    {   
        if(IconImage !=null) IconImage.sprite = inDanger ? IconStates[currentReaction] : IconDefaultState;
    }
}
