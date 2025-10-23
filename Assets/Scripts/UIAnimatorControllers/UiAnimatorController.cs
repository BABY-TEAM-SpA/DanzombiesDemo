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
            PlayAll();
    }

    private void OnDisable()
    {
        StopAll();
    }

    private void OnDestroy()
    {
        StopAll();
    }

    public void PlayAll()
    {
        StopAll();
        if (target == null) target = GetComponent<RectTransform>();
        runningCoroutine = StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        if (sequence != null)
            yield return sequence.Play(this, target);
        runningCoroutine = null;
    }

    public void StopAll()
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
    }
}