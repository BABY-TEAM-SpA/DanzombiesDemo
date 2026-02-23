using System;
using System.Collections;
using UnityEngine;

public static class UiTween
{
    public static IEnumerator Value(MonoBehaviour host, double duration, Action<double> onUpdate, UiEasingType easing, bool scaleTime = true)
    {
        if (host == null || onUpdate == null) yield break;

        double time = 0d;
        while (time < duration)
        {
            if (host == null) yield break;
            
            if (scaleTime) time += Time.deltaTime;
            else time += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01((float)(time / duration));
            onUpdate(UiEasing.Evaluate(easing, t));
            yield return null;
        }

        onUpdate(UiEasing.Evaluate(easing, 1f));
    }
}