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
    protected double beatTime=1d;

    private void OnEnable()
    {
        BeatManager.OnUpdateEvent += OnUpdateSongEvent;
        BeatManager.OnPreBeat += OnPreBeatEvent;
        BeatManager.OnBeat += OnBeatEvent;
        BeatManager.OnPostBeat += OnPostBeatEvent;
        

    }

    private void OnDisable()
    {
        BeatManager.OnUpdateEvent -= OnUpdateSongEvent;
        BeatManager.OnPreBeat -= OnPreBeatEvent;
        BeatManager.OnBeat -= OnBeatEvent;
        BeatManager.OnPostBeat -= OnPostBeatEvent;
    }
    
    private void OnUpdateSongEvent(double beatDuration)
    {
        beatTime = beatDuration;
        OnUpdateSongAction();
    }

    private void OnPauseEvent()//double beatDuration)
    {
        OnPauseSongAction();
    }

    private void OnResumeEvent()//double beatDuration)
    {
        OnResumeAction();
    }
    private void OnPreBeatEvent(int counter)//(BeatType type)
    {
        /*if (type == myBeatType)
        {
            PreBeatAction();
        }*/
        PreBeatAction(counter);
    }
    
    private void OnBeatEvent(int counter ) //(BeatType type)
    {
        /*if (type == myBeatType)
        {
            BeatAction();
        }*/
        BeatAction(counter);
    }

    private void OnPostBeatEvent(int counter) //(BeatType type)
    {
        /*if (type == myBeatType)
        {
            PostBeatAction();
        }*/
        PostBeatAction(counter);
    }

    private void OnStopEvent()//double beatDuration)
    {
        OnStopSongAction();
    }
    
    ///////////--- virtual Actions Management ---///////////
    public virtual void OnUpdateSongAction()//double beatDuration)
    {
        /// implement when Song Plays;
    }
    public virtual void OnPauseSongAction()
    {
        /// implement when Song Pause;
    }

    public virtual void OnResumeAction()
    {
        /// implement when Song Resume;
    }
    public virtual void PreBeatAction(int counter)
    {
        /// implement On: the beat-margen
    }
    public virtual void BeatAction(int counter)
    {
        /// implement On: the beat;
    }
    public virtual void PostBeatAction(int counter)
    {
        /// implement On: the beat+margen;
    }
    public virtual void OnStopSongAction()
    {
        /// implement when song Stops
    }
}
