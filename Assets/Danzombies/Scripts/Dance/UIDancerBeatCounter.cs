using TMPro;
using UnityEngine;

public class UIDancerBeatCounter : Dancer
{
    string[] mapa = { "3", "2", "1", "go!" };
    [SerializeField] TMP_Text feedbackText;
    [SerializeField] UiAnimator animator;

    public override void OnPreDanceStepAction(int beat, BeatManager.BeatType beatType, DanceStep danceStep)
    {
        //Debug.Log("OnPreDanceStepAction UI");
        feedbackText.text = (BeatManager.Instance.globalUpperBar-BeatManager.Instance.localBeatCount)>0 ? 
            (BeatManager.Instance.globalUpperBar - BeatManager.Instance.localBeatCount).ToString()
            : "go";
        animator.PlaySequence("Dance");
    }


    public override void OnDanceStepAction(int beat, BeatManager.BeatType beatType, DanceStep danceStep)
    {
        //Debug.Log(BeatManager.Instance.localBeatCount);
        feedbackText.text = BeatManager.Instance? BeatManager.Instance.localBeatCount.ToString():"1";
        animator.PlaySequence("Dance");
    }

    public override void OnEnablePuzzle(RhythmPuzzle puzzl)
    {
        base.OnEnablePuzzle(puzzl);
        feedbackText.gameObject.SetActive(true);
    }

    public override void OnDisablePuzzle(RhythmPuzzle puzzl)
    {
        Debug.Log("OnDisableText");
        feedbackText.gameObject.SetActive(false);
        base.OnDisablePuzzle(puzzl);
        
    }
}
