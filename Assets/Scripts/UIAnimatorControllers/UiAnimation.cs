using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class UiAnimation
{
    public bool Unscaled = false;
    public enum UiAnimType { Move,MoveTo, Rotate, Scale, Fade, Color}
    public UiAnimType animationType = UiAnimType.Move;
    
    public RectTransform target;
    
    [Header("Values")]
    public Vector3 to;

    [Header("Timing")]
    public UiEasingType easing = UiEasingType.Linear;
    
    private Coroutine runningCoroutine;

    public IEnumerator Play(MonoBehaviour host,float duration)
    {
        Vector3 from;
        if (target == null || host == null)
            yield break;
        
        switch (animationType)
        {
            case UiAnimType.Move:
                from = (Vector3)target.anchoredPosition;
                runningCoroutine = host.StartCoroutine(UiTween.Value(
                    host, duration, t =>
                    {
                        target.anchoredPosition = Vector3.LerpUnclamped(from, from+to, t);
                    }, easing));
                break;
            case UiAnimType.MoveTo:
                from= (Vector3)target.anchoredPosition;
                runningCoroutine = host.StartCoroutine(UiTween.Value(
                    host, duration, t =>
                    {
                        target.anchoredPosition = Vector3.LerpUnclamped(from, to, t);
                    }, easing));
                break;
            case UiAnimType.Rotate:
                Quaternion fromRotation = target.rotation;
                runningCoroutine = host.StartCoroutine(UiTween.Value(
                    host, duration, t =>
                    {
                        target.rotation = Quaternion.LerpUnclamped(fromRotation, Quaternion.Euler(to), t);
                    },easing));
                break;
            case UiAnimType.Scale:
                from = (Vector3)target.localScale;
                runningCoroutine = host.StartCoroutine(UiTween.Value(
                    host, duration, t =>
                    {
                        target.localScale = Vector3.LerpUnclamped(from, to, t);
                    }, easing));
                break;

            case UiAnimType.Fade:
                var graphic = target.GetComponent<CanvasGroup>();
                if (graphic != null)
                {
                    float startAlpha = graphic.alpha;
                    float endAlpha = to.x;
                    float initialColor = graphic.alpha;

                    runningCoroutine = host.StartCoroutine(UiTween.Value(
                        host, duration, t =>
                        {
                            float alpha = Mathf.LerpUnclamped(startAlpha, endAlpha, t);
                            graphic.alpha = alpha;
                        }, easing));
                }
                break;
            case UiAnimType.Color:
                var render = target.GetComponent<Image>();
                if (render != null)
                {
                    Vector3 startColor = new Vector4(render.color.r, render.color.g, render.color.b);
                    runningCoroutine = host.StartCoroutine(UiTween.Value(
                        host, duration, t =>
                        {
                            Vector3 current = Vector3.LerpUnclamped(startColor, to, t);
                            render.color = new Color(current.x, current.y, current.z, 1f);
                        }, easing));
                }
                break;
        }

        yield return new WaitForSecondsRealtime(duration);
    }

    public void Stop(MonoBehaviour host)
    {
        if (runningCoroutine != null && host != null)
        {
            host.StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
    }
}
