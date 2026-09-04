using TMPro;
using UnityEngine;

public class UIDancerBeatCounter : Dancer
{
    [SerializeField] TMP_Text feedbackText;
    [SerializeField] UiAnimator animator;
    
    public override void OnDanceStepAction(int beat, BeatManager.BeatType beatType, DanceStep danceStep)
    {
        //Debug.Log(BeatManager.Instance.localBeatCount);
        feedbackText.text = BeatManager.Instance? BeatManager.Instance.localBeatCount.ToString():"1";
        animator.PlaySequence("Dance");
    }
    
}
