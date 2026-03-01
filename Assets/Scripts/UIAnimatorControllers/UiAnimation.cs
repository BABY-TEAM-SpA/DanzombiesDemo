using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UiTimeMode
{
    Scaled,
    Unscaled,
    DSP
}
[System.Serializable]
public class UiAnimation
{
    [SerializeField] private bool revertOnComplete = false;
    [SerializeField] private RectTransform target;
    [SerializeField] private List<AnimationStep> steps = new();
    

    public RectTransform Target => target;

    private struct TransformState
    {
        public Vector2 anchoredPosition;
        public Vector3 localScale;
        public Quaternion localRotation;
        public float alpha;
        public Color color;
        public bool hasCanvasGroup;
        public bool hasGraphic;
    }

    private TransformState initialState;

    public IEnumerator Play()
    {
        if (!target || steps == null || steps.Count == 0)
            yield break;

        CaptureInitialState();

        foreach (var step in steps)
            yield return step.Play(target);

        if (revertOnComplete)
            RestoreInitialState();
    }

    private void CaptureInitialState()
    {
        initialState.anchoredPosition = target.anchoredPosition;
        initialState.localScale = target.localScale;
        initialState.localRotation = target.localRotation;

        if (target.TryGetComponent(out CanvasGroup cg))
        {
            initialState.alpha = cg.alpha;
            initialState.hasCanvasGroup = true;
        }
        else
        {
            initialState.hasCanvasGroup = false;
        }

        if (target.TryGetComponent(out Graphic g))
        {
            initialState.color = g.color;
            initialState.hasGraphic = true;
        }
        else
        {
            initialState.hasGraphic = false;
        }
    }

    private void RestoreInitialState()
    {
        target.anchoredPosition = initialState.anchoredPosition;
        target.localScale = initialState.localScale;
        target.localRotation = initialState.localRotation;

        if (initialState.hasCanvasGroup &&
            target.TryGetComponent(out CanvasGroup cg))
        {
            cg.alpha = initialState.alpha;
        }

        if (initialState.hasGraphic &&
            target.TryGetComponent(out Graphic g))
        {
            g.color = initialState.color;
        }
    }

    [System.Serializable]
    public class AnimationStep
    {
        public enum UiStepType
        {
            Move,
            MoveTo,
            Rotate,
            Scale,
            Fade,
            Color
        }

        public UiStepType stepType;
        public double duration = 0.3d;
        public UiEasingType easing = UiEasingType.Linear;
        public UiTimeMode timeMode = UiTimeMode.Unscaled;

        public Vector2 position;
        public Vector3 scale = Vector3.one;
        public Vector3 rotation;
        [Range(0f, 1f)] public float alpha = 1f;
        public Color color = Color.white;

        public IEnumerator Play(RectTransform target)
        {
            if (!target)
                yield break;

            if (duration <= 0f)
            {
                ApplyInstant(target);
                yield break;
            }

            Vector2 startPos = target.anchoredPosition;
            Vector3 startScale = target.localScale;
            Quaternion startRot = target.localRotation;

            CanvasGroup canvasGroup = null;
            if (stepType == UiStepType.Fade)
            {
                canvasGroup = target.GetComponent<CanvasGroup>();
                if (!canvasGroup)
                    canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
            }

            float startAlpha = canvasGroup ? canvasGroup.alpha : 0f;

            Graphic graphic = null;
            if (stepType == UiStepType.Color)
                graphic = target.GetComponent<Graphic>();

            Color startColor = graphic ? graphic.color : Color.white;

            double startDspTime = AudioSettings.dspTime;
            double elapsed = 0f;

            while (elapsed < duration)
            {
                switch (timeMode)
                {
                    case UiTimeMode.Scaled:
                        elapsed += Time.deltaTime;
                        break;

                    case UiTimeMode.Unscaled:
                        elapsed += Time.unscaledDeltaTime;
                        break;

                    case UiTimeMode.DSP:
                        elapsed = (float)(AudioSettings.dspTime - startDspTime);
                        break;
                }

                float t = Mathf.Clamp01((float)elapsed / (float)duration);
                t = UiEasing.Evaluate(easing, t);

                Apply(target, t, startPos, startScale, startRot, startAlpha, startColor);

                yield return null;
            }

            Apply(target, 1f, startPos, startScale, startRot, startAlpha, startColor);
        }

        private void ApplyInstant(RectTransform target)
        {
            switch (stepType)
            {
                case UiStepType.Move:
                    target.anchoredPosition += position;
                    break;

                case UiStepType.MoveTo:
                    target.anchoredPosition = position;
                    break;

                case UiStepType.Scale:
                    target.localScale = scale;
                    break;

                case UiStepType.Rotate:
                    target.localRotation = Quaternion.Euler(rotation);
                    break;

                case UiStepType.Fade:
                    if (target.TryGetComponent(out CanvasGroup cg))
                        cg.alpha = alpha;
                    break;

                case UiStepType.Color:
                    if (target.TryGetComponent(out Graphic g))
                        g.color = color;
                    break;
            }
        }

        private void Apply(
            RectTransform target,
            float t,
            Vector2 startPos,
            Vector3 startScale,
            Quaternion startRot,
            float startAlpha,
            Color startColor)
        {
            switch (stepType)
            {
                case UiStepType.Move:
                    target.anchoredPosition =
                        Vector2.LerpUnclamped(startPos, startPos + position, t);
                    break;

                case UiStepType.MoveTo:
                    target.anchoredPosition =
                        Vector2.LerpUnclamped(startPos, position, t);
                    break;

                case UiStepType.Scale:
                    target.localScale =
                        Vector3.LerpUnclamped(startScale, scale, t);
                    break;

                case UiStepType.Rotate:
                    target.localRotation =
                        Quaternion.LerpUnclamped(startRot, Quaternion.Euler(rotation), t);
                    break;

                case UiStepType.Fade:
                    if (target.TryGetComponent(out CanvasGroup cg))
                        cg.alpha = Mathf.LerpUnclamped(startAlpha, alpha, t);
                    break;

                case UiStepType.Color:
                    if (target.TryGetComponent(out Graphic g))
                        g.color = Color.LerpUnclamped(startColor, color, t);
                    break;
            }
        }
    }
}