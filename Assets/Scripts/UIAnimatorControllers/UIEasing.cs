using UnityEngine;
using System;

public enum UiEasingType
{
    Linear,
    EaseInQuad,
    EaseOutQuad,
    EaseInOutQuad,
    EaseInCubic,
    EaseOutCubic,
    EaseInOutCubic,
    EaseInQuart,
    EaseOutQuart,
    EaseInOutQuart,
    EaseBounce,
    EaseElastic,
}

public static class UiEasing
{
    public static float Evaluate(UiEasingType type, float t)
    {
        switch (type)
        {
            default:
            case UiEasingType.Linear: return t;
            case UiEasingType.EaseInQuad: return t * t;
            case UiEasingType.EaseOutQuad: return t * (2f - t);
            case UiEasingType.EaseInOutQuad: return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
            case UiEasingType.EaseInCubic: return t * t * t;
            case UiEasingType.EaseOutCubic: return (--t) * t * t + 1;
            case UiEasingType.EaseInOutCubic: return t < 0.5f ? 4f * t * t * t : (t - 1f) * (2f * t - 2f) * (2f * t - 2f) + 1f;
            case UiEasingType.EaseInQuart: return t * t * t * t;
            case UiEasingType.EaseOutQuart: return 1f - (--t) * t * t * t;
            case UiEasingType.EaseInOutQuart: return t < 0.5f ? 8f * t * t * t * t : 1f - 8f * (--t) * t * t * t;
            case UiEasingType.EaseBounce:{
                const float n1 = 7.5625f;
                const float d1 = 2.75f;

                if (t < 1f / d1) {
                    return n1 * t * t;
                } else if (t < 2f / d1) {
                    return n1 * (t -= 1.5f / d1) * t + 0.75f;
                } else if (t < 2.5f / d1) {
                    return n1 * (t -= 2.25f / d1) * t + 0.9375f;
                } else {
                    return n1 * (t -= 2.625f / d1) * t + 0.984375f;
                }
                
            }
            case UiEasingType.EaseElastic:
            {
                const float c4 = (2f * Mathf.PI) / 3f;

                if (t == 0f)
                    return 0f;
                if (t == 1f)
                    return 1f;

                return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
                
            }
        }
    }
}