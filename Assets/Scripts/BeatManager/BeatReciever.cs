using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static BeatManager;



public abstract class BeatReciever: MonoBehaviour
{
    //public BeatType myBeatType;
    
    ///////////--- Events Management ---///////////

    private void OnEnable()
    {
        BeatManager.OnPlay += OnPlayEvent;
        BeatManager.OnPause += OnPauseEvent;
        BeatManager.OnPreBeat += OnPreBeatEvent;
        BeatManager.OnBeat += OnBeatEvent;
        BeatManager.OnPostBeat += OnPostBeatEvent;
        BeatManager.OnStop += OnStopEvent;
        
    }

    private void OnDisable()
    {
        BeatManager.OnPlay -= OnPlayEvent;
        BeatManager.OnPause -= OnPauseEvent;
        BeatManager.OnPreBeat -= OnPreBeatEvent;
        BeatManager.OnBeat -= OnBeatEvent;
        BeatManager.OnPostBeat -= OnPostBeatEvent;
        BeatManager.OnStop -= OnStopEvent;
    }
    
    private void OnPlayEvent(float beatDuration)
    {
        OnPlaySongAction(beatDuration);
    }

    private void OnPauseEvent(float beatDuration)
    {
        OnPauseSongAction();
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

    private void OnStopEvent(float beatDuration)
    {
        OnStopSongAction();
    }
    
    ///////////--- virtual Actions Management ---///////////
    public virtual void OnPlaySongAction(float beatDuration)
    {
        /// implement when Song Plays;
    }
    public virtual void OnPauseSongAction()
    {
        /// implement when Song Pause;
    }
    public virtual void CompassAction()
    {
        // implement On first beat of Compass
    }
    public virtual void PreBeatAction(int compass)
    {
        /// implement On: the beat-margen
    }
    public virtual void BeatAction(int compass)
    {
        if(compass == 0)CompassAction();
        /// implement On: the beat;
    }
    public virtual void PostBeatAction(int compass)
    {
        /// implement On: the beat+margen;
    }
    public virtual void OnStopSongAction()
    {
        /// implement when song Stops
    }
}
