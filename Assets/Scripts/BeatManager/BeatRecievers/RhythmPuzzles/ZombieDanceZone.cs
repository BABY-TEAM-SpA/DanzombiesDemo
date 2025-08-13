using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieDanceZone : RhythmPuzzle
{
    [SerializeField] private List<ZombieDanceBrain> zombies = new List<ZombieDanceBrain>();
    [SerializeField] private SpriteRenderer zone;
    [SerializeField] private Color[] Colores;
    [SerializeField] private Color[] ColoresPlayer;
    
    
    
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
    public override void BeatAction(int counter, int counterCompass)
    {
        base.BeatAction(counter, counterCompass);
        if (useCompass) zone.color = (player!=null)?ColoresPlayer[counterCompass]:Colores[counterCompass];
        else
        {
            int aux = counter;
            if (ShouldRepeat) aux = counter % Colores.Length;
            zone.color = (player!=null)?ColoresPlayer[aux]:Colores[aux];
        }
    }

    public override void OnPlayerInputAction(DanceStep step)
    {
        base.OnPlayerInputAction(step);
        
    }

    public override void PostBeatAction(int counter, int counterCompass)
    {
        base.PostBeatAction(counter, counterCompass);
        if (playerDanceStep == DanceStep.None && player != null)
        {
            //hacer daño al player
            //player.Hurt();
        }
        
    }
}
