using UnityEngine;

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
    EaseOutBounce,
    EaseOutElastic
}

public static class UiEasing
{
    public static float Evaluate(UiEasingType type, float t)
    {
        t = Mathf.Clamp01(t);

        switch (type)
        {
            default:
            case UiEasingType.Linear: return t;

            case UiEasingType.EaseInQuad: return t * t;
            case UiEasingType.EaseOutQuad: return t * (2f - t);
            case UiEasingType.EaseInOutQuad:
                return t < 0.5f
                    ? 2f * t * t
                    : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;

            case UiEasingType.EaseInCubic: return t * t * t;
            case UiEasingType.EaseOutCubic: return 1f - Mathf.Pow(1f - t, 3f);
            case UiEasingType.EaseInOutCubic:
                return t < 0.5f
                    ? 4f * t * t * t
                    : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;

            case UiEasingType.EaseInQuart: return t * t * t * t;
            case UiEasingType.EaseOutQuart: return 1f - Mathf.Pow(1f - t, 4f);
            case UiEasingType.EaseInOutQuart:
                return t < 0.5f
                    ? 8f * t * t * t * t
                    : 1f - Mathf.Pow(-2f * t + 2f, 4f) / 2f;

            case UiEasingType.EaseOutBounce: return EaseOutBounce(t);
            case UiEasingType.EaseOutElastic: return EaseOutElastic(t);
        }
    }

    private static float EaseOutBounce(float t)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (t < 1f / d1)
            return n1 * t * t;
        else if (t < 2f / d1)
            return n1 * (t -= 1.5f / d1) * t + 0.75f;
        else if (t < 2.5f / d1)
            return n1 * (t -= 2.25f / d1) * t + 0.9375f;
        else
            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
    }

    private static float EaseOutElastic(float t)
    {
        const float c4 = (2f * Mathf.PI) / 3f;

        if (t == 0f) return 0f;
        if (t == 1f) return 1f;

        return Mathf.Pow(2f, -10f * t) *
               Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
    }
}