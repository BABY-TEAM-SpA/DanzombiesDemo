using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieDanceZone : RhythmPuzzle
{
    [SerializeField] private List<ZombieDanceBrain> zombies = new List<ZombieDanceBrain>();

    [SerializeField] private Color[] defaultColors = new Color[] { Color.gray, Color.white };
    [SerializeField] private Color[] PlayerInsideColors = new Color[] { Color.magenta, Color.forestGreen };
    [SerializeField] private Color SuccesColor = Color.yellow;
    [SerializeField] private Color FailureColor = Color.red;

private void Start()
    {
        foreach (ZombieDanceBrain zombie in zombies)
        {
            zombie.Connect(this);
        }
    }

    private void OnDisable()
    {
        foreach (ZombieDanceBrain zombie in zombies)
        {
            zombie.Disconnect(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered");
            this.player = other.GetComponentInChildren<PlayerAnimatorController>();
            //suscribirse a que el player entregue un input
            player.OnDance += OnPlayerInputAction;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.OnDance -= OnPlayerInputAction;
            this.player = null;
        }
    }
    public override void VisualFeedback(int counter, int counterCompass)
    {
        if (!hasRecieveInput)
        {
            if (useCompass) feedBack.color = (player!=null)?PlayerInsideColors[counterCompass]:defaultColors[counterCompass];
            else
            {
                int aux = counter;
                if (ShouldRepeat) aux = counter % defaultColors.Length;
                if(feedBack!=null) feedBack.color = (player!=null)?PlayerInsideColors[aux]:defaultColors[aux];
            }
        }
    }
    public override void PlayerDanceReaction(bool IsPlayerDanceCorrect)
    {
        DanceLevelController.Instance?.ModificarVida(IsPlayerDanceCorrect);
        if (feedBack != null)
        {
            feedBack.color=(IsPlayerDanceCorrect)?SuccesColor:FailureColor;   
        }
    }
    
}
