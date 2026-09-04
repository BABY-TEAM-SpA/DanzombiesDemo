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
    [SerializeField] private Transform objectToAnimate;
    public Transform Target => objectToAnimate;
    [SerializeField] private UiTimeMode timeMode = UiTimeMode.Unscaled;
    [SerializeField] private List<AnimationStep> animationSteps = new();

    private struct TransformState
    {
        public bool isUI;
        public Vector2 anchoredPosition;
        public Vector3 localPosition;
        public Vector3 localScale;
        public Quaternion localRotation;
        public float alpha;
        public Color color;
        public bool hasCanvasGroup;
        public bool hasGraphic;
        public bool hasSpriteRenderer;
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

        if (revertOnComplete) RestoreInitialState();
    }

    private void CaptureInitialState()
    {
        initialState.localPosition = objectToAnimate.localPosition;
        initialState.localScale = objectToAnimate.localScale;
        initialState.localRotation = objectToAnimate.localRotation;

        if (objectToAnimate is RectTransform rect)
        {
            initialState.isUI = true;
            initialState.anchoredPosition = rect.anchoredPosition;
        }

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

        if (objectToAnimate.TryGetComponent(out SpriteRenderer sr))
        {
            initialState.color = sr.color;
            initialState.alpha = sr.color.a;
            initialState.hasSpriteRenderer = true;
        }
    }

    private void RestoreInitialState()
    {
        objectToAnimate.localPosition = initialState.localPosition;
        objectToAnimate.localScale = initialState.localScale;
        objectToAnimate.localRotation = initialState.localRotation;

        if (initialState.isUI && objectToAnimate is RectTransform rect)
            rect.anchoredPosition = initialState.anchoredPosition;

        if (initialState.hasCanvasGroup &&
            objectToAnimate.TryGetComponent(out CanvasGroup cg))
            cg.alpha = initialState.alpha;

        if (initialState.hasGraphic &&
            objectToAnimate.TryGetComponent(out Graphic g))
            g.color = initialState.color;

        if (initialState.hasSpriteRenderer &&
            objectToAnimate.TryGetComponent(out SpriteRenderer sr))
            sr.color = initialState.color;
    }
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

    public IEnumerator Play(Transform target, UiTimeMode timeMode, Func<bool> isCancelled)
    {
        RectTransform rect = target as RectTransform;
        bool isUI = rect != null;
        if (!target || duration <= 0f)
            yield break;

        if (stepType == UiStepType.Wait)
        {
            yield return WaitForDuration(duration, timeMode, isCancelled);
            yield break;
        }

        Vector2 startAnchored = Vector2.zero;
        Vector3 startLocal = target.localPosition;
        if (isUI) startAnchored = rect.anchoredPosition;
        Vector3 startScale = target.localScale;
        Quaternion startRot = target.localRotation;
        SpriteRenderer sprite = null;
        CanvasGroup canvasGroup = null;
        Graphic graphic = null;
        

        if(stepType == UiStepType.Color || stepType == UiStepType.Fade)
        {
            canvasGroup = target.GetComponent<CanvasGroup>();
            if (!canvasGroup) sprite = target.GetComponent<SpriteRenderer>();
            if (!canvasGroup && !sprite) graphic = target.GetComponent<Graphic>();
        }

        if (stepType == UiStepType.Color) graphic = target.GetComponent<Graphic>();

        float startAlpha = 1;
        if(canvasGroup) startAlpha = canvasGroup.alpha;
        else if(sprite) startAlpha = sprite.color.a;
        else if (graphic) startAlpha = graphic.color.a;
        Color startColor = Color.white;

        if(graphic)
            startColor = graphic.color;
        else if(sprite)
            startColor = sprite.color;

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

            Apply(target,isUI,rect,t, startAnchored,startLocal,startScale,startRot,startAlpha,startColor,canvasGroup,graphic,sprite);

            yield return null;
        }

        Apply(target,isUI,rect,1f, startAnchored,startLocal,startScale,startRot,startAlpha,startColor,canvasGroup,graphic,sprite);
    }

    private IEnumerator WaitForDuration(double duration, UiTimeMode mode, Func<bool> isCancelled)
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
        Transform target,
        bool isUI,
        RectTransform rect,
        float t,
        Vector2 startAnchored,
        Vector3 startLocal,
        Vector3 startScale,
        Quaternion startRot,
        float startAlpha,
        Color startColor,
        CanvasGroup canvasGroup,
        Graphic graphic,
        SpriteRenderer sprite){
        switch (stepType)
        {
            case UiStepType.Move:
                if(isUI) rect.anchoredPosition = Vector2.LerpUnclamped( startAnchored, startAnchored + position,t);
                else target.localPosition = Vector3.LerpUnclamped( startLocal, startLocal + (Vector3)position, t);
                break;

            case UiStepType.MoveTo:
                if(isUI) rect.anchoredPosition =Vector2.LerpUnclamped(startAnchored, position, t);
                else target.localPosition = Vector3.LerpUnclamped(startLocal, new Vector3(position.x, position.y, startLocal.z), t);
                break;

            case UiStepType.Scale:
                target.localScale = Vector3.LerpUnclamped(startScale, scale, t);
                break;

            case UiStepType.Rotate:
                target.localRotation = Quaternion.LerpUnclamped(startRot, Quaternion.Euler(rotation), t);
                break;

            case UiStepType.Fade:

                if(canvasGroup) canvasGroup.alpha =Mathf.LerpUnclamped(startAlpha, alpha, t);
                else if(sprite)
                {
                    Color c = sprite.color;
                    c.a = Mathf.LerpUnclamped(startAlpha, alpha, t);
                    sprite.color = c;
                }
                else if (graphic)
                {
                    Color c = graphic.color;
                    c.a = Mathf.LerpUnclamped(startAlpha, alpha, t);
                    graphic.color = c;
                }
                break;

            case UiStepType.Color:

                if(graphic) graphic.color = Color.LerpUnclamped(startColor, color, t);
                else if(sprite) sprite.color = Color.LerpUnclamped(startColor, color, t);

                break;
        }
    }
}