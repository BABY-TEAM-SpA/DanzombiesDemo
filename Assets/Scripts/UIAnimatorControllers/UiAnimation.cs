using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class UiAnimation
{
    public enum UiAnimType { Move, Rotate, Scale, Fade }
    public UiAnimType animationType = UiAnimType.Move;

    [Header("Values")]
    public Vector3 from;
    public Vector3 to;

    [Header("Timing")]
    public UiEasingType easing = UiEasingType.EaseOutQuad;

    

    private Coroutine runningCoroutine;

    public IEnumerator Play(MonoBehaviour host, RectTransform target,float duration)
    {
        if (target == null || host == null)
            yield break;
        
        switch (animationType)
        {
            case UiAnimType.Move:
                runningCoroutine = host.StartCoroutine(UiTween.Value(
                    host, duration, t =>
                    {
                        target.anchoredPosition = Vector3.LerpUnclamped(from, to, t);
                    }, easing));
                break;
            case UiAnimType.Rotate:
                runningCoroutine = host.StartCoroutine(UiTween.Value(
                    host, duration, t =>
                    {
                        target.rotation = Quaternion.LerpUnclamped(Quaternion.Euler(from), Quaternion.Euler(to), t);
                    },easing));
                break;
            case UiAnimType.Scale:
                runningCoroutine = host.StartCoroutine(UiTween.Value(
                    host, duration, t =>
                    {
                        target.localScale = Vector3.LerpUnclamped(from, to, t);
                    }, easing));
                break;

            case UiAnimType.Fade:
                var graphic = target.GetComponent<Graphic>();
                if (graphic != null)
                {
                    float startAlpha = from.x;
                    float endAlpha = to.x;
                    Color initialColor = graphic.color;

                    runningCoroutine = host.StartCoroutine(UiTween.Value(
                        host, duration, t =>
                        {
                            float alpha = Mathf.LerpUnclamped(startAlpha, endAlpha, t);
                            graphic.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
                        }, easing));
                }
                break;
        }

        yield return new WaitForSeconds(duration);
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
