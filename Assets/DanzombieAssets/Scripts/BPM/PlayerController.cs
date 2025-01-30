using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : BeatReciever
{
    public bool reciveInput=false;
    private bool inputDetectedBeforeCheck = false; 
    public Queue<string> inputRegist;

    //public static PlayerInputReciever Instance { get; private set; }

    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
        /*
        if (Instance != null && Instance != this) 
        { 
            Destroy(this.gameObject); 
        } 
        else 
        { 
            Instance = this; 
        }*/
    }
    public void Update()
    {
       //        
    }

    public override void PreBeatAction()
    {
        //inputRegist = 0;
        reciveInput = true;
    }
    public override void BeatAction()
    {
        //
    }

    public override void PostBeatAction()
    { 
        //
        //inputRegist = 0;
        reciveInput = false;
    }

  

    
}
