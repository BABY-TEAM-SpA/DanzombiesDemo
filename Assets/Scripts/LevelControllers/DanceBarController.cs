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
    }

    public void Activate(bool isActive)
    {
        UpdateFlowBars(PlayerManager.Player.GetFlowDamage(0));
        uiAnimator.PlaySequence(isActive ? "Open" : "Close");
    }
    
    
    public void UpdateFlowBars(int value, bool isInside =false)
    {
        if (value <= 2) currentReaction = 2;
        else if (value <= 5) currentReaction = 1;
        else currentReaction = 0;
        
        foreach (Image barra in FlowBars)
        {
            barra.fillAmount = value/10f;
            barra.color = barReactions[currentReaction];
            beatBarMaterial.SetFloat("_RainbowEnabled", value==10?1f:0f);
        }
        UpdateIconFeedback(isInside);
    }
    public void UpdateIconFeedback(bool inDanger)
    {
        if (inDanger)
        {
            if(IconImage !=null) IconImage.sprite = IconStates[currentReaction];
        }
        else
        {
            if(IconImage !=null) IconImage.sprite =  IconDefaultState;
            beatBarMaterial.SetFloat("_RainbowEnabled", 0f);
        }
        if(IconImage !=null) IconImage.sprite = inDanger ? IconStates[currentReaction] : IconDefaultState;
    }
}
