using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UiAnimatorController : MonoBehaviour
{
    [SerializeField] private RectTransform target;
    [SerializeField] private bool playOnEnable = false;
    [SerializeField] public UiAnimationSequence sequence = new UiAnimationSequence();

    private Coroutine runningCoroutine;
    private readonly List<UiAnimation> activeAnimations = new List<UiAnimation>();

    private void OnEnable()
    {
        if (target == null) target = GetComponent<RectTransform>();
    }

    private void Start()
    {
        if (playOnEnable)
            PlaySequence();
    }

    private void OnDisable() => StopSequence();
    private void OnDestroy() => StopSequence();

    public void PlaySequence()
    {
        StopSequence();
        if (target == null) target = GetComponent<RectTransform>();
        runningCoroutine = StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        if (sequence != null)
            yield return sequence.Play(this, activeAnimations);
        runningCoroutine = null;
    }

    public void StopSequence()
    {
        // Detiene la coroutine raíz
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }

        // Detiene todas las animaciones hijas
        foreach (var anim in activeAnimations)
            anim.Stop(this);

        activeAnimations.Clear();
    }
}