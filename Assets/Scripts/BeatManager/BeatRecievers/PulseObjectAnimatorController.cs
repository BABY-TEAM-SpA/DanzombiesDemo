using UnityEngine;

public class PulseObjectAnimatorController : BeatReciever
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
            DanceAnimator.SetTrigger("Play");
        }
    }
    public override void OnPlaySongAction(float beatDuration)
    {
        SetBeatDuration(beatDuration);
    }

    public override void OnPauseSongAction()
    {
        SetBeatDuration(0f);
        //DanceAnimator.SetTrigger("Stop");
    }

    public override void BeatAction(int counter, int counterCompass)
    {
        DanceAnimator.SetTrigger("OnBeat");
    }
    public override void OnStopSongAction()
    {
        DanceAnimator.SetTrigger("Stop");
    }
}
