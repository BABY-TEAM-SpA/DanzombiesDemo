using System;
using System.Collections;
using UnityEngine;

public static class UiTween
{
    public static IEnumerator Value(MonoBehaviour host, float duration, Action<float> onUpdate, UiEasingType easing, bool ignoreTimeScale = false)
    {
        if (host == null || onUpdate == null) yield break;

        float time = 0f;
        while (time < duration)
        {
            if (host == null) yield break;

            if (ignoreTimeScale) time += Time.unscaledDeltaTime;
            else time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            onUpdate(UiEasing.Evaluate(easing, t));
            yield return null;
        }

        onUpdate(UiEasing.Evaluate(easing, 1f));
    }
}