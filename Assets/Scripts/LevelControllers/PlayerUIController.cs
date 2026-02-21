using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class Heart
{
    public Image HeartContainer;
    public Sprite HealedHeart;
    public Sprite DamageHeart;

    public void SetSprite(bool isHealed = true)
    {
        HeartContainer.sprite = isHealed? HealedHeart: DamageHeart;
    }
}


public class PlayerUIController : MonoBehaviour
{
    [Header("Player Lifes")]
    [SerializeField] private Image playerHead;
    [SerializeField] private Sprite[] playerReacctions;

    [SerializeField] private List<Heart> hearts = new List<Heart>();


    public static PlayerUIController Instance { get; private set; }

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
    public void UpdatePlayerHead()
    {
    
    } 

    public void UpdateLifesPlayer(int lifes, int player = 0)
    {
        Heart heart = hearts[lifes];
        heart.HeartContainer.sprite = heart.DamageHeart;
        
    }
}
