using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class UiAnimatorController : MonoBehaviour
{
    [SerializeField] private RectTransform target;
    [SerializeField] private bool playOnEnable = false;
    [SerializeField] private UiAnimationSequence sequence = new UiAnimationSequence();

    private Coroutine runningCoroutine;

    private void OnEnable()
    {
        if(target == null) target = GetComponent<RectTransform>();
        if (playOnEnable)
            PlaySequence();
    }

    private void OnDisable()
    {
        StopSequence();
    }

    private void OnDestroy()
    {
        StopSequence();
    }

    public void PlaySequence()
    {
        StopSequence();
        if (target == null) target = GetComponent<RectTransform>();
        runningCoroutine = StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        if (sequence != null)
            yield return sequence.Play(this);
        runningCoroutine = null;
    }

    public void StopSequence()
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
    }
}