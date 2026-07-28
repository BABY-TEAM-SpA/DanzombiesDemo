using System.Collections;
using UnityEngine;

public class InteReactableFeedback : FeedbackElement
{
    #region [VARIABLES]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Inactive")]
    [SerializeField] private Color inactiveColor;
    [SerializeField] private Color activeColor;

    private Coroutine pulseRoutine;
    private Coroutine hideRoutine;
    #endregion

    #region [METHODS]
    public void Show() => spriteRenderer.color = activeColor;
    public void Hide()
    {
        if (hideRoutine != null)
            return;

        hideRoutine = StartCoroutine(HideRoutine());
    }

    public void Animate()
    {
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            transform.localScale = Vector3.one;
        }
        pulseRoutine = StartCoroutine(PulseRoutine(transform));
    }
    #endregion

    #region [COROUTINES]
    private IEnumerator PulseRoutine(Transform target)
    {
        const float PULSE_FACTOR = 0.75f;
        const float PULSE_DURATION = 0.1f;

        Vector3 prevScale = transform.localScale;
        Vector3 nextScale = prevScale * PULSE_FACTOR;

        // 1/2 <¬
        float elapsed = 0f;
        while (elapsed < PULSE_DURATION / 2f)
        {
            elapsed += Time.deltaTime;
            target.localScale = Vector3.Lerp(prevScale, nextScale, elapsed / (PULSE_DURATION / 2f));
            yield return null;
        }
        transform.localScale = nextScale;

        // 2/2 <¬
        elapsed = 0f;
        while (elapsed < PULSE_DURATION / 2f)
        {
            elapsed += Time.deltaTime;
            target.localScale = Vector3.Lerp(nextScale, prevScale, elapsed / (PULSE_DURATION / 2f));
            yield return null;
        }
        transform.localScale = prevScale;

        pulseRoutine = null;
    }

    private IEnumerator HideRoutine()
    {
        yield return new WaitWhile(() => pulseRoutine != null);
        spriteRenderer.color = inactiveColor;
        hideRoutine = null;
    }
    #endregion

    public override void Activate(bool isActive)
    {
        
    }
}
