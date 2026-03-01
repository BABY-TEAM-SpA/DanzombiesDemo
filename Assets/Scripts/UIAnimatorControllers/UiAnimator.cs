using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiAnimator : MonoBehaviour
{
    [SerializeField] private List<UiAnimationSequence> sequences = new();

    private Coroutine runningCoroutine;

    public void PlaySequence(string sequenceName)
    {
        StopSequence();

        var sequence = sequences.Find(s => s.SequenceName == sequenceName);
        if (sequence == null)
        {
            Debug.LogWarning($"Sequence '{sequenceName}' not found.");
            return;
        }

        runningCoroutine = StartCoroutine(sequence.Play());
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