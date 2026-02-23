using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UiAnimationSequence
{
    public string SequenceName;
    
    [Serializable]
    public class SequenceStep
    {
        public enum StepType { Wait, Animation }
        public StepType stepType = StepType.Animation;
        public float duration = 0f;
        public List<UiAnimation> animations = new List<UiAnimation>();
        public UnityEvent onStepComplete;
    }

    [SerializeField] private List<SequenceStep> steps = new List<SequenceStep>();
    [SerializeField] private bool loop = false;
    [SerializeField] private int loopCount = 1;
    [SerializeField] public UnityEvent onSequenceStart;
    [SerializeField] public UnityEvent onSequenceComplete;

    public IEnumerator Play(MonoBehaviour host, List<UiAnimation> activeAnimations)
    {
        int count = loop ? Mathf.Max(1, loopCount) : 1;

        for (int i = 0; i < count; i++)
        {
            onSequenceStart.Invoke();

            foreach (var step in steps)
            {
                switch (step.stepType)
                {
                    case SequenceStep.StepType.Wait:
                        yield return new WaitForSecondsRealtime(step.duration);
                        break;

                    case SequenceStep.StepType.Animation:
                        int remaining = step.animations.Count;
                        if (remaining == 0) yield break;

                        foreach (var anim in step.animations)
                        {
                            // Registrar la animación activa
                            if (!activeAnimations.Contains(anim))
                                activeAnimations.Add(anim);

                            host.StartCoroutine(PlayAndNotify(host, anim, step.duration, () =>
                            {
                                remaining--;
                                activeAnimations.Remove(anim);
                            }));
                        }

                        while (remaining > 0)
                            yield return null;
                        break;
                }

                step.onStepComplete?.Invoke();
            }

            onSequenceComplete.Invoke();
        }
    }

    private IEnumerator PlayAndNotify(MonoBehaviour host, UiAnimation anim, float duration, Action onComplete)
    {
        yield return anim.Play(host, duration);
        onComplete?.Invoke();
    }
}
