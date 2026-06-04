using System;
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
    [SerializeField] private RectTransform objectToAnimate;
    [SerializeField] private UiTimeMode timeMode = UiTimeMode.Unscaled;
    [SerializeField] private List<AnimationStep> animationSteps = new();

    public RectTransform Target => objectToAnimate;

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

    public IEnumerator Play(Func<bool> isCancelled)
    {
        if (!objectToAnimate || animationSteps == null || animationSteps.Count == 0)
            yield break;

        CaptureInitialState();

        foreach (var step in animationSteps)
        {
            if (isCancelled()) yield break;
            yield return step.Play(objectToAnimate, timeMode, isCancelled);
        }

        if (revertOnComplete)
            RestoreInitialState();
    }

    private void CaptureInitialState()
    {
        initialState.anchoredPosition = objectToAnimate.anchoredPosition;
        initialState.localScale = objectToAnimate.localScale;
        initialState.localRotation = objectToAnimate.localRotation;

        if (objectToAnimate.TryGetComponent(out CanvasGroup cg))
        {
            initialState.alpha = cg.alpha;
            initialState.hasCanvasGroup = true;
        }

        if (objectToAnimate.TryGetComponent(out Graphic g))
        {
            initialState.color = g.color;
            initialState.hasGraphic = true;
        }
    }

    private void RestoreInitialState()
    {
        objectToAnimate.anchoredPosition = initialState.anchoredPosition;
        objectToAnimate.localScale = initialState.localScale;
        objectToAnimate.localRotation = initialState.localRotation;

        if (initialState.hasCanvasGroup &&
            objectToAnimate.TryGetComponent(out CanvasGroup cg))
            cg.alpha = initialState.alpha;

        if (initialState.hasGraphic &&
            objectToAnimate.TryGetComponent(out Graphic g))
            g.color = initialState.color;
    }

    [System.Serializable]
    public class AnimationStep
    {
        public enum UiStepType
        {
            Wait,
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

        public Vector2 position;
        public Vector3 scale = Vector3.one;
        public Vector3 rotation;
        [Range(0f, 1f)] public float alpha = 1f;
        public Color color = Color.white;

        public IEnumerator Play(
            RectTransform target,
            UiTimeMode timeMode,
            Func<bool> isCancelled)
        {
            if (!target || duration <= 0f)
                yield break;

            if (stepType == UiStepType.Wait)
            {
                yield return WaitForDuration(duration, timeMode, isCancelled);
                yield break;
            }

            Vector2 startPos = target.anchoredPosition;
            Vector3 startScale = target.localScale;
            Quaternion startRot = target.localRotation;

            CanvasGroup canvasGroup = null;
            Graphic graphic = null;

            if (stepType == UiStepType.Fade)
            {
                canvasGroup = target.GetComponent<CanvasGroup>();
                if (!canvasGroup)
                    canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
            }

            if (stepType == UiStepType.Color)
                graphic = target.GetComponent<Graphic>();

            float startAlpha = canvasGroup ? canvasGroup.alpha : 0f;
            Color startColor = graphic ? graphic.color : Color.white;

            double startDsp = AudioSettings.dspTime;
            double elapsed = 0;

            while (elapsed < duration)
            {
                if (isCancelled()) yield break;

                switch (timeMode)
                {
                    case UiTimeMode.Scaled:
                        elapsed += Time.deltaTime;
                        break;

                    case UiTimeMode.Unscaled:
                        elapsed += Time.unscaledDeltaTime;
                        break;

                    case UiTimeMode.DSP:
                        elapsed = AudioSettings.dspTime - startDsp;
                        break;
                }

                float t = Mathf.Clamp01((float)(elapsed / duration));
                t = UiEasing.Evaluate(easing, t);

                Apply(target, t, startPos, startScale, startRot,
                      startAlpha, startColor, canvasGroup, graphic);

                yield return null;
            }

            Apply(target, 1f, startPos, startScale, startRot,
                  startAlpha, startColor, canvasGroup, graphic);
        }

        private IEnumerator WaitForDuration(
            double duration,
            UiTimeMode mode,
            Func<bool> isCancelled)
        {
            double startDsp = AudioSettings.dspTime;
            double elapsed = 0;

            while (elapsed < duration)
            {
                if (isCancelled()) yield break;

                switch (mode)
                {
                    case UiTimeMode.Scaled:
                        elapsed += Time.deltaTime;
                        break;

                    case UiTimeMode.Unscaled:
                        elapsed += Time.unscaledDeltaTime;
                        break;

                    case UiTimeMode.DSP:
                        elapsed = AudioSettings.dspTime - startDsp;
                        break;
                }

                yield return null;
            }
        }

        private void Apply(
            RectTransform target,
            float t,
            Vector2 startPos,
            Vector3 startScale,
            Quaternion startRot,
            float startAlpha,
            Color startColor,
            CanvasGroup canvasGroup,
            Graphic graphic)
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
                    if (canvasGroup)
                        canvasGroup.alpha =
                            Mathf.LerpUnclamped(startAlpha, alpha, t);
                    break;

                case UiStepType.Color:
                    if (graphic)
                        graphic.color =
                            Color.LerpUnclamped(startColor, color, t);
                    break;
            }
        }
    }
}