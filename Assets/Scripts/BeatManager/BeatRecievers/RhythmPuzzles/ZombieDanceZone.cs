using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieDanceZone : RhythmPuzzle
{
    [SerializeField] private List<ZombieDanceBrain> zombies = new List<ZombieDanceBrain>();

    [SerializeField] private Color[] defaultColors = new Color[] { Color.gray, Color.white };
    
    [SerializeField] private Color[] PlayerInsideColors = new Color[] { Color.magenta, Color.forestGreen };

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
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
        {
            PlayerEnter(player);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
        {
            PlayerLeave(player);
        }
    }
    public override void VisualFeedbackToPlayerDance(bool isCorrect)
    {
        /*int aux = counter;
        if (ShouldRepeat) aux = counter % defaultColors.Length;
        if(feedBack!=null) feedBack.color = (playersInside.Count>0)?PlayerInsideColors[aux]:defaultColors[aux];*/
        // Esto va a cambiar cuando usemos shader
    }


    public override void ReactToPlayersDance(PlayerManager player, DanceStep step)
    {
        if (step == DanceStep.None)
        {
            return;
        }
        float flow = player.GetFlowDamage(player.saveDanceStep != step);
        if (flow < GameManager.Alza)
        {
            PlayerHasNoFlow(player);
        }
    }

    public override void PlayerHasNoFlow(PlayerManager player)
    {
        player.GetLifeDamage(true);
        PlayerLeave(player);
    }
    
    public override void PlayerEnter(PlayerManager player)
    {
        base.PlayerEnter(player);
        LevelUIController.Instance?.UpdateZombieFeedbackUI(true);
        
    }

    public override void PlayerLeave(PlayerManager player)
    {
        base.PlayerLeave(player);
        LevelUIController.Instance?.UpdateZombieFeedbackUI(false);
        
    }
    


}
