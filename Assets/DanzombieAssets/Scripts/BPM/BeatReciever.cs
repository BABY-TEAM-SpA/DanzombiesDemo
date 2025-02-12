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
        BeatManager.OnPreBeat += OnPreBeatEvent;
        BeatManager.OnPostBeat += OnPostBeatEvent;
        //LevelController.OnPauseEvent += OnPauseEventReceiver;
    }

    private void OnDisable()
    {
        //BeatManager.OnHalfBeat -= HalfBeatAction;
        BeatManager.OnPlay -= OnPlayEvent;
        BeatManager.OnBeat -= OnBeatEvent;
        BeatManager.OnPreBeat -= OnPreBeatEvent;
        BeatManager.OnPostBeat -= OnPostBeatEvent;
        //LevelController.OnPauseEvent -= OnPauseEventReceiver;
    }
    
    //Event Reactions

    private void OnPlayEvent(float beatDuration)
    {
        OnPlaySongAction(beatDuration);
    }
    private void OnPreBeatEvent()//(BeatType type)
    {
        /*if (type == myBeatType)
        {
            PreBeatAction();
        }*/
        PreBeatAction();
    }
    
    private void OnBeatEvent()//(BeatType type)
    {
        /*if (type == myBeatType)
        {
            BeatAction();
        }*/
        BeatAction();
    }

    private void OnPostBeatEvent()//(BeatType type)
    {
        /*if (type == myBeatType)
        {
            PostBeatAction();
        }*/
        PostBeatAction();

    }

    public virtual void OnPlaySongAction(float beatDuration)
    {

    }

    public virtual void PreBeatAction()
    {
        /// implement 
    }

    public virtual void BeatAction()
    {
        /// implement 
    }
    
    public virtual void PostBeatAction()
    {
        /// implement 
    }

    public virtual void HalfBeatAction()
    {
        
    }
    
}
