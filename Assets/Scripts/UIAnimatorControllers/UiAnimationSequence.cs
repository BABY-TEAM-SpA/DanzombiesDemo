using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UiAnimationSequence
{
    [Header("Sequence Settings")]
    [SerializeField] private string sequenceName;
    [SerializeField] private bool loop;
    [SerializeField] private int loopCount = 1;
    [Header("Animations")]
    [SerializeField] private List<UiAnimation> animations = new();
    [Header("Sequence Events")]
    public UnityEvent OnSequenceStart;
    public UnityEvent OnSequenceComplete;

    public string SequenceName => sequenceName;

    public IEnumerator Play()
    {
        if (animations == null || animations.Count == 0)
            yield break;

        int iterations = loop ? Mathf.Max(1, loopCount) : 1;

        for (int i = 0; i < iterations; i++)
        {
            OnSequenceStart?.Invoke();

            List<IEnumerator> running = new();

            foreach (var anim in animations)
                running.Add(anim.Play());

            bool runningAny = true;

            while (runningAny)
            {
                runningAny = false;

                for (int j = 0; j < running.Count; j++)
                {
                    if (running[j] != null)
                    {
                        if (!running[j].MoveNext())
                            running[j] = null;
                        else
                            runningAny = true;
                    }
                }

                yield return null;
            }

            OnSequenceComplete?.Invoke();
        }
    }
}