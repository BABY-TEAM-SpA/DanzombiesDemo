using System.Collections.Generic;
using UnityEngine;

public class PulseObjectAnimatorController : BeatReciever
{
    [SerializeField] List<Animator> DanceAnimators = new List<Animator>();
    private float currentBeatOnPlayer = 0f;
    [SerializeField] AnimatorOverrideController animatorOverrideController;

    public void Start()
    {
        if (BeatManager.Instance != null)
        {
            SetBeatDuration(BeatManager.Instance.beatDuration);
            if (animatorOverrideController != null)
            {
                foreach (Animator animator in DanceAnimators)
                {
                    animator.runtimeAnimatorController = animatorOverrideController;
                    animator.enabled = true;
                }
            }
        }
    }

    private void SetBeatDuration(float duration)
    {
        currentBeatOnPlayer = duration;
        base.OnPlaySongAction(currentBeatOnPlayer);
        foreach(Animator DanceAnimator in DanceAnimators)
        {
            DanceAnimator.enabled = true;
            DanceAnimator.SetFloat("Beat",(1f/currentBeatOnPlayer));
            //DanceAnimator.SetTrigger("OnBeat");
            DanceAnimator.Play("Pulse");
        }
    }
    public override void OnPlaySongAction(float beatDuration)
    {
        SetBeatDuration(beatDuration);
        
    }

    public override void OnPauseSongAction()
    {
        //SetBeatDuration(0f);
        foreach(Animator DanceAnimator in DanceAnimators)
        {
            DanceAnimator.enabled = false;
        }
    }

    public override void BeatAction(int counter, int counterCompass)
    {
        foreach(Animator DanceAnimator in DanceAnimators)
        {
            //DanceAnimator.SetTrigger("OnBeat");
            DanceAnimator.Play("Pulse");
        }
    }
   
}
