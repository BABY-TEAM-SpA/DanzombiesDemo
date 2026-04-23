using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static BeatManager;



public abstract class BeatReciever: MonoBehaviour
{
    public bool isActive { set; get; } = true;
    
    ///////////--- Events Management ---///////////
    protected double barTime=1d;
    public bool isOnBeat { set; get; } = false;
    

    private void OnEnable()
    {
        BeatManager.OnUpdateEvent += OnUpdateSongEvent;
        BeatManager.OnPreBeat += OnPreBeatEvent;
        BeatManager.OnBeat += OnBeatEvent;
        BeatManager.OnPostBeat += OnPostBeatEvent;
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    private void OnDisable()
    {
        BeatManager.OnUpdateEvent -= OnUpdateSongEvent;
        BeatManager.OnPreBeat -= OnPreBeatEvent;
        BeatManager.OnBeat -= OnBeatEvent;
        BeatManager.OnPostBeat -= OnPostBeatEvent;
    }
    
    private void OnUpdateSongEvent(double barDuration)
    {
        barTime = barDuration;
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
        if(isActive)PreBeatAction(counter);
        isOnBeat = true;
    }
    
    private void OnBeatEvent(int counter ) //(BeatType type)
    {
        /*if (type == myBeatType)
        {
            BeatAction();
        }*/
        if(isActive)BeatAction(counter);
    }

    private void OnPostBeatEvent(int counter) //(BeatType type)
    {
        /*if (type == myBeatType)
        {
            PostBeatAction();
        }*/
        if(isActive)PostBeatAction(counter);
        isOnBeat=false;
    }

    private void OnStopEvent()//double beatDuration)
    {
        OnStopSongAction();
    }
    
    ///////////--- virtual Actions Management ---///////////
    public abstract void OnUpdateSongAction();//double beatDuration)

    public abstract void OnPauseSongAction();

    public abstract void OnResumeAction();

    public abstract void PreBeatAction(int counter);

    public abstract void BeatAction(int counter);

    public abstract void PostBeatAction(int counter);
    public abstract void OnStopSongAction();

}
