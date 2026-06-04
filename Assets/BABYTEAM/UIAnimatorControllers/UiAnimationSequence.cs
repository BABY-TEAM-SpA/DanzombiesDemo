using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UiAnimationSequence
{
    [SerializeField] private string sequenceName;
    [SerializeField] private bool loop;
    [SerializeField] private int loopCount = 1;

    public UnityEvent OnSequenceStart;
    public UnityEvent OnSequenceComplete;

    [SerializeField] private List<UiAnimation> animationChannels = new();

    public string SequenceName => sequenceName;

    public IEnumerator Play(MonoBehaviour host, Func<bool> isCancelled)
    {
        if (animationChannels == null || animationChannels.Count == 0)
            yield break;

        int iterations = loop ? Mathf.Max(1, loopCount) : 1;

        for (int i = 0; i < iterations; i++)
        {
            if (isCancelled()) yield break;

            OnSequenceStart?.Invoke();

            int remaining = animationChannels.Count;

            foreach (var anim in animationChannels)
            {
                host.StartCoroutine(RunChannel(anim, isCancelled, () => remaining--));
            }

            while (remaining > 0)
            {
                if (isCancelled()) yield break;
                yield return null;
            }

            OnSequenceComplete?.Invoke();
        }
    }

    private IEnumerator RunChannel(
        UiAnimation anim,
        Func<bool> isCancelled,
        Action onComplete)
    {
        yield return anim.Play(isCancelled);
        onComplete?.Invoke();
    }
}