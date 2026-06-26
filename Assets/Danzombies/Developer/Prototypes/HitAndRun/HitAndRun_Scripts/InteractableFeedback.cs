using System.Collections;
using UnityEngine;

public class InteractableFeedback : MonoBehaviour
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

    public void Pulse()
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
    private IEnumerator PulseRoutine(Transform target, float factor = 0.75f, float duration = 0.2f)
    {
        
        Vector3 prevScale = transform.localScale;
        Vector3 nextScale = prevScale * factor;

        // Primera mitad
        float elapsed = 0f;

        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            target.localScale = Vector3.Lerp(prevScale, nextScale, elapsed / (duration / 2f));
            yield return null;
        }
        transform.localScale = nextScale;

        // Segunda mitad
        elapsed = 0f;

        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            target.localScale = Vector3.Lerp(nextScale, prevScale, elapsed / (duration / 2f));
            yield return null;
        }
        transform.localScale = prevScale;

        pulseRoutine = null;
    }

    /// <summary>
    /// Para esperar a que 
    /// </summary>
    private IEnumerator HideRoutine()
    {
        yield return new WaitWhile(() => pulseRoutine != null);
        spriteRenderer.color = inactiveColor;
        hideRoutine = null;
    }
    #endregion
}
