using System;
using UnityEngine;

public class PlayerBeatReciever : BeatReciever
{
    [SerializeField] Animator DanceAnimator;
    private float currentBeatOnPlayer = 0f;

    public void Start()
    {
        SetBeatDuration(BeatManager.Instance.beatDuration);
    }

    private void SetBeatDuration(float duration)
    {
        currentBeatOnPlayer = duration;
        base.OnPlaySongAction(currentBeatOnPlayer);
        if (DanceAnimator != null)
        {
            Debug.Log("Playing Dance Animator");
            DanceAnimator.SetFloat("Beat",(1f/currentBeatOnPlayer));
        }
    }
    public override void OnPlaySongAction(float beatDuration)
    {
        SetBeatDuration(beatDuration);
    }

    public override void BeatAction(int compass)
    {
        base.BeatAction(compass);
        DanceAnimator.SetTrigger("Idle");
        Invoke("ResetIdle",0.1f);
    }

    public override void CompassAction()
    {
        
        
    }

    public void ResetIdle()
    {
        DanceAnimator.ResetTrigger("Idle");
    }
}
