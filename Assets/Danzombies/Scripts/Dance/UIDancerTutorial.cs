using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIDancerTutorial : Dancer
{
    #region [VARIABLES]
    [SerializeField] private DanceIcon danceIcon;
    private DanceIcon.SchemesSpritesControls currentDanceScheme = new DanceIcon.SchemesSpritesControls();
    [SerializeField] private DanceIcon leftDirIcon;
    private DanceIcon.SchemesSpritesControls currentLeftScheme = new DanceIcon.SchemesSpritesControls();
    [SerializeField] private DanceIcon rightDirIcon;
    private DanceIcon.SchemesSpritesControls currentRightScheme = new DanceIcon.SchemesSpritesControls();
    
    [SerializeField] PlayerInput _playerInput;
    private string currentScheme;
    
    [SerializeField] private CanvasGroup danceCanvas;
    
    [SerializeField, Range(0f, 1f)] private float incomingAlpha;

    private DanceStep futureDanceStep = DanceStep.None;
    #endregion

    #region [UNITY]
    public void Start()
    {
        PrepareUI();
    }
    #endregion

    #region [METHODS]
    #region Dancer - Activation
    public override void OnEnablePuzzle(RhythmPuzzle puzzl)
    {
        SetActiveCanvas(true);
    }

    public override void OnDisablePuzzle(RhythmPuzzle puzzl)
    {
        SetActiveCanvas(false);
    }
    #endregion

    #region Dancer - Steps
    public override void OnPrepareStepAction(int beat, BeatManager.BeatType beatType, DanceStep step)
    {
        currentDanceStep = step;
        if (!danceCanvas.isActiveAndEnabled) return;
        RefreshIcons(false, false, false);
    }

    public override void OnSetNextSetAction(int beat, BeatManager.BeatType beatType, DanceStep step)
    {
        futureDanceStep = step;
        if (!danceCanvas.isActiveAndEnabled) return;
        RefreshIcons(false, false, false);
    }

    public override void OnDanceStepAction(int beat, BeatManager.BeatType beatType, DanceStep step)
    {
        currentDanceStep = step;
        Debug.Log($"CurrentDanceStep: {currentDanceStep} | FutureDanceStep: {futureDanceStep}");

        if (!danceCanvas.isActiveAndEnabled) return;
        RefreshIcons(true, true, true);

        if (currentDanceStep == DanceStep.None) return;
        danceIcon.animator.PlaySequence("Pulse");
        if (GetLean(currentDanceStep) == "R") rightDirIcon.animator.PlaySequence("Pulse");
        else leftDirIcon.animator.PlaySequence("Pulse");
    }

    public override void OnReleaseStepAction(int beat, BeatManager.BeatType beatType, DanceStep step)
    {
        //currentDanceStep = DanceStep.None;
        if (!danceCanvas.isActiveAndEnabled) return;
        RefreshIcons(false, false, false);
    }
    #endregion

    #region UI
    public void PrepareUI()
    {
        currentScheme = _playerInput.currentControlScheme;

        currentDanceScheme = danceIcon.schemes.FirstOrDefault(x => x.schemeName == currentScheme);
        danceIcon.iconFXRenderer.sprite = currentDanceScheme.spriteFX;
        danceIcon.iconFXRenderer.SetNativeSize();

        currentLeftScheme = leftDirIcon.schemes.FirstOrDefault(x => x.schemeName == currentScheme);
        leftDirIcon.iconFXRenderer.sprite = currentLeftScheme.spriteFX;
        leftDirIcon.iconFXRenderer.SetNativeSize();

        currentRightScheme = rightDirIcon.schemes.FirstOrDefault(x => x.schemeName == currentScheme);
        rightDirIcon.iconFXRenderer.sprite = currentRightScheme.spriteFX;
        rightDirIcon.iconFXRenderer.SetNativeSize();

        RefreshIcons(false, false, false);
        danceIcon.iconRenderer.SetNativeSize();
        leftDirIcon.iconRenderer.SetNativeSize();
        rightDirIcon.iconRenderer.SetNativeSize();
    }

    private void RefreshIcons(bool danceIconPressed, bool leftPressed, bool rightPressed)
    {
        bool currentDanceMatches = currentDanceStep != DanceStep.None;
        bool futureDanceMatches = futureDanceStep != DanceStep.None;

        DanceStep previewStep = currentDanceMatches
            ? currentDanceStep : futureDanceMatches
                ? futureDanceStep : DanceStep.None;

        danceIcon.iconRenderer.color = ResolveAlpha(currentDanceMatches, futureDanceMatches);
        danceIcon.iconRenderer.sprite = ResolveSprite(currentDanceScheme, GetOrientation(previewStep), danceIconPressed && currentDanceMatches);

        string currentLean = GetLean(currentDanceStep);
        string futureLean = GetLean(futureDanceStep);

        bool currentIsRight = currentLean == "R";
        bool futureIsRight = futureLean == "R";
        rightDirIcon.iconRenderer.color = ResolveAlpha(currentIsRight, futureIsRight);
        rightDirIcon.iconRenderer.sprite = ResolveSprite(currentRightScheme, "", rightPressed && currentIsRight);

        bool currentIsLeft = currentLean == "L";
        bool futureIsLeft = futureLean == "L";
        leftDirIcon.iconRenderer.color = ResolveAlpha(currentIsLeft, futureIsLeft);
        leftDirIcon.iconRenderer.sprite = ResolveSprite(currentLeftScheme, "", leftPressed && currentIsLeft);
    }

    public void SetActiveCanvas(bool active)
    {
        PrepareUI();
        danceCanvas.gameObject.SetActive(active);
        danceIcon.iconFXRenderer.gameObject.SetActive(active);
        danceIcon.iconRenderer.gameObject.SetActive(active);
        leftDirIcon.iconRenderer.gameObject.SetActive(active);
        leftDirIcon.iconFXRenderer.gameObject.SetActive(active);
        rightDirIcon.iconRenderer.gameObject.SetActive(active);
        rightDirIcon.iconFXRenderer.gameObject.SetActive(active);
    }
    #endregion

    #region Helpers
    private static string GetLean(DanceStep step) => step == DanceStep.None ? "" : step.ToString().Substring(0, 1);
    private static string GetOrientation(DanceStep step) => step == DanceStep.None ? "" : step.ToString().Substring(2);

    private Color ResolveAlpha(bool currentMatches, bool futureMatches)
    {
        if (currentMatches) return Color.white;
        if (futureMatches) return new Color(1, 1, 1, incomingAlpha);
        return Color.clear;
    }

    private Sprite ResolveSprite(DanceIcon.SchemesSpritesControls scheme, string orientation, bool pressed)
    {
        DanceIcon.SchemesSpritesControls.ControlButtons button = string.IsNullOrEmpty(orientation)
            ? (scheme.buttons.Count > 0 ? scheme.buttons[0] : null)
            : scheme.buttons.Find(x => x.buttonName == orientation);
        if (button == null) return scheme.defaultSprite;
        return pressed ? button.pressed : button.active;
    }
    #endregion
    #endregion

    [Serializable]
    public class DanceIcon
    {
        [Serializable]
        public class SchemesSpritesControls
        {
            
            [Serializable]
            public class ControlButtons
            {
                public string buttonName;
                public Sprite active;
                public Sprite pressed;
            }
            public string schemeName;
            public Sprite spriteFX;
            public Sprite defaultSprite;
            public List<ControlButtons> buttons = new List<ControlButtons>();
        }

        public UiAnimator animator;
        public Image iconRenderer;
        public Image iconFXRenderer;
        public List<SchemesSpritesControls> schemes = new List<SchemesSpritesControls>();
    }
}

