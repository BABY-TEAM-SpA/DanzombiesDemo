using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelUIController : MonoBehaviour
{
    [Header("Barras de Flow")] 
    [SerializeField] private Image IconImage;
    private int currentReaction = 0;
    [SerializeField] private Sprite IconDefaultState;
    [SerializeField] private Sprite[] IconStates = new Sprite[] {};
    [SerializeField] private List<Image> FlowBars = new List<Image>();
    [SerializeField] private Color[] barReactions = new Color[] {};
    public static LevelUIController Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

    }

    public void UpdateZombieFeedbackUI(bool inDanger)
    {   
        if(IconImage !=null) IconImage.sprite = inDanger ? IconStates[currentReaction] : IconDefaultState;
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
        UpdateZombieFeedbackUI(isInside);
        
    }

    
}
