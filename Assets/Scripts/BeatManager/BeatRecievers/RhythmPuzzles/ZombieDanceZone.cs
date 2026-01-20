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
            player.RemoveTargetPuzzle(this);
            playersInside.Remove(player);
        }
    }
    public override void VisualFeedback(int counter, int counterCompass)
    {
        int aux = counter;
        if (ShouldRepeat) aux = counter % defaultColors.Length;
        if(feedBack!=null) feedBack.color = (playersInside.Count>0)?PlayerInsideColors[aux]:defaultColors[aux];
        // Esto va a cambiar cuando usemos shader
    }

    public override void OnRhythmPuzzleBeatReaction()
    {
        foreach (PlayerManager player in playersInside)
        {
            if (currentPuzzleStep != DanceStep.None)
            {
                player.GetFlowDamage(player.saveDanceStep != currentPuzzleStep);
            }
        }
    }
    
    public override void PlayerEnter(PlayerManager player)
    {
        base.PlayerEnter(player);
        DanceLevelController.Instance.UpdateZombieFeedbackUI(true);
        
    }

    public override void PlayerLeave(PlayerManager player)
    {
        base.PlayerLeave(player);
        DanceLevelController.Instance.UpdateZombieFeedbackUI(false);
        
    }
    


}
