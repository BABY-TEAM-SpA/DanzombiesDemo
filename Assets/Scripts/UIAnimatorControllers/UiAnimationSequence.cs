using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UiAnimationSequence
{
    [Serializable]
    public class SequenceStep
    {
        public enum StepType
        {
            Wait,
            Animation,
            
        }
        public StepType stepType = StepType.Animation;
        public float duration = 0f;
        public List<UiAnimation> animations = new List<UiAnimation>();
        public UnityEvent onStepComplete;
        
    }

    [SerializeField] private List<SequenceStep> steps = new List<SequenceStep>();
    [SerializeField] private bool loop = false;
    [SerializeField] private int loopCount = 1;
    [SerializeField] public UnityEvent onSequenceStart;
    

    public IEnumerator Play(MonoBehaviour host)
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
                        yield return new WaitForSeconds(step.duration);
                        break;

                    case SequenceStep.StepType.Animation:
                        int remaining = step.animations.Count;
                        if (remaining == 0) break;

                        foreach (var anim in step.animations)
                        {
                            host.StartCoroutine(PlayAndNotify(host, anim, step.duration,() => remaining--));
                        }

                        // Espera hasta que todas terminen
                        yield return new WaitUntil(() => remaining <= 0);
                        break;
                }
                step.onStepComplete?.Invoke();
            }
        }
        
    }

    private IEnumerator PlayAndNotify(MonoBehaviour host, UiAnimation anim, float duration,Action onComplete)
    {
        yield return anim.Play(host, duration);
        onComplete?.Invoke();
    }
}
