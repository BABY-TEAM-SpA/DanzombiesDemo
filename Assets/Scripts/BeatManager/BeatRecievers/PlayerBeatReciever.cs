using System;
using UnityEngine;

public class PlayerBeatReciever : BeatReciever
{
    [SerializeField] Animator DanceAnimator;
    private float currentBeatOnPlayer = 0f;

    public void Start()
    {
        if(BeatManager.Instance!=null) SetBeatDuration(BeatManager.Instance.beatDuration);
    }

    private void SetBeatDuration(float duration)
    {
        currentBeatOnPlayer = duration;
        base.OnPlaySongAction(currentBeatOnPlayer);
        if (DanceAnimator != null)
        {
            //Debug.Log("Playing Dance Animator");
            DanceAnimator.SetFloat("Beat",(1f/currentBeatOnPlayer));
        }
    }
    public override void OnPlaySongAction(float beatDuration)
    {
        DanceAnimator.enabled = true;
        SetBeatDuration(beatDuration);
    }

    public override void BeatAction(int counter, int counterCompass)
    {
        base.BeatAction(counter, counterCompass);
        DanceAnimator.SetTrigger("Idle");
        //Invoke("ResetIdle",0.1f);
    }

    public override void CompassAction()
    {
        
        
    }

    public override void OnPauseSongAction()
    {
        DanceAnimator.enabled = false;
    }

    public void ResetIdle()
    {
        DanceAnimator.ResetTrigger("Idle");
    }
}
