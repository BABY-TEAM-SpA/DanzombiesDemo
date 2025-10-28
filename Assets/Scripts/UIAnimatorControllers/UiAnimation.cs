using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class UiAnimation
{
    public bool Unscaled = false;
    public enum UiAnimType { Move,MoveTo, Rotate, Scale, Fade }
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
                    }, easing,Unscaled));
                break;
            case UiAnimType.MoveTo:
                from= (Vector3)target.anchoredPosition;
                runningCoroutine = host.StartCoroutine(UiTween.Value(
                    host, duration, t =>
                    {
                        target.anchoredPosition = Vector3.LerpUnclamped(from, to, t);
                    }, easing,Unscaled));
                break;
            case UiAnimType.Rotate:
                Quaternion fromRotation = target.rotation;
                runningCoroutine = host.StartCoroutine(UiTween.Value(
                    host, duration, t =>
                    {
                        target.rotation = Quaternion.LerpUnclamped(fromRotation, Quaternion.Euler(to), t);
                    },easing,Unscaled));
                break;
            case UiAnimType.Scale:
                from = (Vector3)target.localScale;
                runningCoroutine = host.StartCoroutine(UiTween.Value(
                    host, duration, t =>
                    {
                        target.localScale = Vector3.LerpUnclamped(from, to, t);
                    }, easing,Unscaled));
                break;

            case UiAnimType.Fade:
                var graphic = target.GetComponent<Graphic>();
                if (graphic != null)
                {
                    float startAlpha = graphic.color.a;
                    float endAlpha = to.x;
                    Color initialColor = graphic.color;

                    runningCoroutine = host.StartCoroutine(UiTween.Value(
                        host, duration, t =>
                        {
                            float alpha = Mathf.LerpUnclamped(startAlpha, endAlpha, t);
                            graphic.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
                        }, easing,Unscaled));
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
