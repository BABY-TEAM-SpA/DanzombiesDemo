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
    EaseInQuint,
    EaseOutQuint,
    EaseInOutQuint
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
            case UiEasingType.EaseInQuint: return t * t * t * t * t;
            case UiEasingType.EaseOutQuint: return 1f + (--t) * t * t * t * t;
            case UiEasingType.EaseInOutQuint: return t < 0.5f ? 16f * t * t * t * t * t : 1f + 16f * (--t) * t * t * t * t;
        }
    }
}