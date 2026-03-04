using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiAnimator : MonoBehaviour
{
    [SerializeField] private List<UiAnimationSequence> sequences = new();

    private Coroutine currentSequence;
    private bool isCancelling;

    public void PlaySequence(string sequenceName)
    {
        StopSequence();

        var sequence = sequences.Find(s => s.SequenceName == sequenceName);
        if (sequence == null)
        {
            //Debug.LogWarning($"Sequence '{sequenceName}' not found.");
            return;
        }

        isCancelling = false;
        currentSequence = StartCoroutine(RunSequence(sequence));
    }

    public void PlaySequence(int index = 0)
    {
        StopSequence();
        UiAnimationSequence sequence = null;
        if (index >= 0 && index < sequences.Count)
        {
            sequence = sequences[index];
        }
        
        if (sequence == null)
        {
            Debug.LogWarning($"Sequence '{index}' not found.");
            return;
        }
        isCancelling = false;
        currentSequence = StartCoroutine(RunSequence(sequence));
    }

    private IEnumerator RunSequence(UiAnimationSequence sequence)
    {
        yield return sequence.Play(this, () => isCancelling);
        currentSequence = null;
    }

    public void StopSequence()
    {
        isCancelling = true;

        if (currentSequence != null)
        {
            StopCoroutine(currentSequence);
            currentSequence = null;
        }

        StopAllCoroutines();
    }

    private void OnDisable()
    {
        StopSequence();
    }

    private void OnDestroy()
    {
        StopSequence();
    }
}