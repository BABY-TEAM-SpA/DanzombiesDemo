using System;
using System.Collections;
using UnityEngine;

public static class UiTween
{
    public static IEnumerator Value(
        MonoBehaviour host,
        double duration,
        Action<double> onUpdate,
        UiEasingType easing,
        bool useScaledTime = true)
    {
        if (!host || onUpdate == null)
            yield break;

        // Duración 0 → aplicar valor final inmediato
        if (duration <= 0d)
        {
            onUpdate(UiEasing.Evaluate(easing, 1f));
            yield break;
        }

        double time = 0d;

        while (time < duration)
        {
            if (!host) yield break;

            time += useScaledTime
                ? Time.deltaTime
                : Time.unscaledDeltaTime;

            float t = Mathf.Clamp01((float)(time / duration));
            double eased = UiEasing.Evaluate(easing, t);

            onUpdate(eased);

            yield return null;
        }

        // Asegurar valor final exacto
        onUpdate(UiEasing.Evaluate(easing, 1f));
    }
}