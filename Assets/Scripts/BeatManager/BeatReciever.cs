using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static BeatManager;



public abstract class BeatReciever: MonoBehaviour
{
    //public BeatType myBeatType;

    private void OnEnable()
    {
        //BeatManager.OnHalfBeat += HalfBeatAction;
        
        BeatManager.OnPlay += OnPlayEvent;
        BeatManager.OnBeat += OnBeatEvent;
        //BeatManager.OnCompass += OnCompassStartEvent;
        BeatManager.OnPreBeat += OnPreBeatEvent;
        BeatManager.OnPostBeat += OnPostBeatEvent;
        //LevelController.OnPauseEvent += OnPauseEventReceiver;
    }

    private void OnDisable()
    {
        //BeatManager.OnHalfBeat -= HalfBeatAction;
        BeatManager.OnPlay -= OnPlayEvent;
        BeatManager.OnBeat -= OnBeatEvent;
        //BeatManager.OnCompass -= OnCompassStartEvent;
        BeatManager.OnPreBeat -= OnPreBeatEvent;
        BeatManager.OnPostBeat -= OnPostBeatEvent;
        //LevelController.OnPauseEvent -= OnPauseEventReceiver;
    }
    
    //Event Reactions

    private void OnPlayEvent(float beatDuration)
    {
        OnPlaySongAction(beatDuration);
    }
    private void OnPreBeatEvent(int compass)//(BeatType type)
    {
        /*if (type == myBeatType)
        {
            PreBeatAction();
        }*/
        PreBeatAction(compass);
    }
    
    private void OnBeatEvent(int compass) //(BeatType type)
    {
        /*if (type == myBeatType)
        {
            BeatAction();
        }*/
        BeatAction(compass);
    }

    private void OnPostBeatEvent(int compass) //(BeatType type)
    {
        /*if (type == myBeatType)
        {
            PostBeatAction();
        }*/
        PostBeatAction(compass);

    }

    public virtual void OnPlaySongAction(float beatDuration)
    {

    }

    public virtual void PreBeatAction(int compass)
    {
        /// implement 
    }

    
    public virtual void BeatAction(int compass)
    {
        if(compass == 0)CompassAction();
        /// implement 
    }
    
    public virtual void CompassAction()
    {
        // implement OnFirstBeat of Compass
    }
    
    public virtual void PostBeatAction(int compass)
    {
        /// implement 
    }
    
    
}
