using UnityEngine;

[ExecuteAlways] // Permite ejecutarse también en Edit Mode
[RequireComponent(typeof(Animator))]
public class PulseObjectAnimatorController : BeatReciever
{
    [Header("Animator Settings")]
    [SerializeField] private Animator danceAnimator;
    [SerializeField] private RuntimeAnimatorController controllerToAssign;

    private float currentBeatOnPlayer = 0f;

    private void OnValidate()
    {
        EnsureAnimator();
    }

    private void Awake()
    {
        EnsureAnimator();
    }

    private void EnsureAnimator()
    {
        if (danceAnimator == null)
        {
            danceAnimator = GetComponent<Animator>();
            if (danceAnimator == null)
            {
                danceAnimator = gameObject.AddComponent<Animator>();
            }
        }

        if (controllerToAssign != null && danceAnimator.runtimeAnimatorController != controllerToAssign)
        {
            danceAnimator.runtimeAnimatorController = controllerToAssign;
        }
    }

    private void Start()
    {
        if (!Application.isPlaying) return;

        SetBeatDuration(.5f);
        if (BeatManager.Instance != null)
        {
            SetBeatDuration(BeatManager.Instance.beatDuration);
        }
    }

    private void SetBeatDuration(float duration)
    {
        currentBeatOnPlayer = duration;

        base.OnPlaySongAction(currentBeatOnPlayer);

        if (danceAnimator != null && currentBeatOnPlayer > 0f)
        {
            danceAnimator.SetFloat("Beat", 1f / currentBeatOnPlayer);
            danceAnimator.SetTrigger("OnBeat");
        }
    }

    public override void OnPlaySongAction(float beatDuration)
    {
        SetBeatDuration(beatDuration);
    }

    public override void OnPauseSongAction()
    {
        SetBeatDuration(0f);
    }

    public override void BeatAction(int counter, int counterCompass)
    {
        if (danceAnimator != null)
            danceAnimator.SetTrigger("OnBeat");
    }
}
