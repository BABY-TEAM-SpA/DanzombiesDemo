using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class DanceLevelController : MonoBehaviour
{

    [Header("Barras de Flow")] 
    [SerializeField] private Image zombieEye;
    private int currentReaction = 0;
    [SerializeField] private Sprite zombieClosedEye;
    [SerializeField] private Sprite[] zombieReactions = new Sprite[] {};
    [SerializeField] private List<Image> ZombieBars = new List<Image>();
    [SerializeField] private Color[] barReactions = new Color[] {};
    
    [Header("Player Lifes")]
    [SerializeField] private List<Image> hearts = new List<Image>();

    [SerializeField] private Image GregHead;
    [SerializeField] private Sprite[] spriteGregHappy;

    public UnityEvent OnPlayerDies;
    public static DanceLevelController Instance { get; private set; }

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
        if(zombieEye !=null) zombieEye.sprite = inDanger ? zombieReactions[currentReaction] : zombieClosedEye;
    }
    
    public void UpdateFlowBars(float value)
    {
        if (value <= 2f) currentReaction = 2;
        else if (value <= 5f) currentReaction = 1;
        else currentReaction = 0;
        
        if(zombieEye !=null) zombieEye.sprite = zombieReactions[currentReaction];
        
        foreach (Image barra in ZombieBars)
        {
            barra.fillAmount = value/10;
            barra.color = barReactions[currentReaction];
        }
    }

    public void UpdateLifesPlayer(int lifes, int player = 0)
    {
        Debug.Log("Update Lifes");
    }
}
