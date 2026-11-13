using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.Switch;
using UnityEngine.UI;

public class UIDancerTutorial : Dancer
{
    #region [VARIABLES]
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
        public Image VfxIconRenderer;
        public Image futureIconRenderer;
        public Image currentIconRenderer;
        
        public List<SchemesSpritesControls> schemes = new List<SchemesSpritesControls>();
    }
    
    [SerializeField] private bool debug = false;
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
    private void OnEnable()
    {
        if (_playerInput != null)
            _playerInput.onControlsChanged += HandleControlsChanged;
    }

    private void OnDisable()
    {
        if (_playerInput != null)
            _playerInput.onControlsChanged -= HandleControlsChanged;
    }

    private void HandleControlsChanged(PlayerInput input)
    {
        PrepareUI();
    }
    #endregion

    #region [METHODS]
    #region Dancer - Activation
    public override void OnEnablePuzzle(RhythmPuzzle puzzle)
    {
        currentDanceStep = DanceStep.None;
        futureDanceStep = DanceStep.None;
        SetFutureDanceStep();
        SetDanceStep();
        
    }

    public override void OnDisablePuzzle(RhythmPuzzle puzzle)
    {
        currentDanceStep = DanceStep.None;
        futureDanceStep = DanceStep.None;
        SetFutureDanceStep();
        SetDanceStep();
        
    }

    public override void OnReseatPuzzle(RhythmPuzzle puzzle)
    {
        currentDanceStep = DanceStep.None;
        futureDanceStep = DanceStep.None;
        if (danceCanvas.isActiveAndEnabled) SetDanceStep();
    }
    #endregion

    #region Dancer - Steps

    public override void OnSetNextSetAction(int beat, BeatManager.BeatType beatType, DanceStep step)
    {
        futureDanceStep = step;
        if (!danceCanvas.isActiveAndEnabled) return;
        SetFutureDanceStep();
        
    }

    public override void OnDanceStepAction(int beatCounter, BeatManager.BeatType beatType, int beatTimeMs, DanceStep danceStep)
    {
        currentDanceStep = danceStep;
        if (!danceCanvas.isActiveAndEnabled) return;
        if (beatType == BeatManager.BeatType.SecondEmpty)
        {
            OnReleaseStepAction(beatCounter, beatType, danceStep);
            return;
        }
        SetDanceStep();
    }

    public  void OnReleaseStepAction(int beat, BeatManager.BeatType beatType, DanceStep step)
    {
        //currentDanceStep = DanceStep.None;
        if (!danceCanvas.isActiveAndEnabled) return;
        //SetDanceStep(futureDanceStep);
    }
    #endregion

    #region UI
    private string GetDetailedSchemeName()
    {
        string baseScheme = _playerInput.currentControlScheme;
        if(debug) Debug.Log($"Debug scheme name: {baseScheme}");
        if (baseScheme == "Gamepad")
        {
            var gamepad = _playerInput.devices.OfType<Gamepad>().FirstOrDefault() ?? Gamepad.current;
            if(debug) Debug.Log($"Gamepad Setted with: {gamepad} controll");
            switch (gamepad)
            {
                case DualShockGamepad:
                    return "PlayStation";
                case XInputController:
                    return "Xbox";
                case SwitchProControllerHID:
                    return "Switch";
                default:
                    return "Xbox";
            }
        }
        return baseScheme;
    }
    
    public void PrepareUI()
    {
        currentScheme = GetDetailedSchemeName();

        currentDanceScheme = danceIcon.schemes.FirstOrDefault(x => x.schemeName == currentScheme);
        danceIcon.currentIconRenderer.sprite = currentDanceScheme.spriteFX;
        danceIcon.currentIconRenderer.SetNativeSize();

        currentLeftScheme = leftDirIcon.schemes.FirstOrDefault(x => x.schemeName == currentScheme);
        leftDirIcon.currentIconRenderer.sprite = currentLeftScheme.spriteFX;
        leftDirIcon.currentIconRenderer.SetNativeSize();

        currentRightScheme = rightDirIcon.schemes.FirstOrDefault(x => x.schemeName == currentScheme);
        rightDirIcon.currentIconRenderer.sprite = currentRightScheme.spriteFX;
        rightDirIcon.currentIconRenderer.SetNativeSize();
        
        danceIcon.currentIconRenderer.SetNativeSize();
        leftDirIcon.currentIconRenderer.SetNativeSize();
        rightDirIcon.currentIconRenderer.SetNativeSize();
    }

    public void SetFutureDanceStep()
    {
        if(debug) Debug.Log($"FutureDance {futureDanceStep} ");
        if(futureDanceStep == DanceStep.None)
        {
            danceIcon.futureIconRenderer.gameObject.SetActive(false);
            leftDirIcon.futureIconRenderer.gameObject.SetActive(false);
            rightDirIcon.futureIconRenderer.gameObject.SetActive(false);
            return;
        }
        string orientation = GetOrientation(futureDanceStep);
        string lean = GetLean(futureDanceStep);
        
        danceIcon.futureIconRenderer.sprite = ResolveSprite(currentDanceScheme, orientation,false);
        danceIcon.futureIconRenderer.SetNativeSize();
        danceIcon.futureIconRenderer.gameObject.SetActive(true);
        
        rightDirIcon.futureIconRenderer.sprite = ResolveSprite(currentRightScheme,"",false);
        rightDirIcon.futureIconRenderer.SetNativeSize();
        rightDirIcon.futureIconRenderer.gameObject.SetActive(lean =="R");
        
        leftDirIcon.futureIconRenderer.sprite = ResolveSprite(currentLeftScheme,"",false);
        leftDirIcon.futureIconRenderer.SetNativeSize();
        leftDirIcon.futureIconRenderer.gameObject.SetActive(lean =="L");
    }
    public void SetDanceStep()
    {
        if(currentDanceStep == DanceStep.None)
        {
            danceIcon.currentIconRenderer.gameObject.SetActive(false);
            leftDirIcon.currentIconRenderer.gameObject.SetActive(false);
            rightDirIcon.currentIconRenderer.gameObject.SetActive(false);
            return;
        }

        if(debug) Debug.Log($"Dance {currentDanceStep}");
        string orientation = GetOrientation(currentDanceStep);
        string lean = GetLean(currentDanceStep);
        
        danceIcon.currentIconRenderer.sprite = ResolveSprite(currentDanceScheme, orientation,true);
        danceIcon.currentIconRenderer.SetNativeSize();
        danceIcon.currentIconRenderer.gameObject.SetActive(true);
        
        rightDirIcon.currentIconRenderer.sprite = ResolveSprite(currentRightScheme,"",true);
        rightDirIcon.currentIconRenderer.SetNativeSize();
        rightDirIcon.currentIconRenderer.gameObject.SetActive(lean =="R");
        
        leftDirIcon.currentIconRenderer.sprite = ResolveSprite(currentLeftScheme,"",true);
        leftDirIcon.currentIconRenderer.SetNativeSize();
        leftDirIcon.currentIconRenderer.gameObject.SetActive(lean =="L");
        AnimateDanceStep(currentDanceStep);
    }

    private void AnimateDanceStep(DanceStep danceStep)
    {
        if(debug) Debug.Log($"AnimateDanceStep {danceStep}");
        danceIcon.VfxIconRenderer.transform.localScale = Vector3.zero;
        rightDirIcon.VfxIconRenderer.transform.localScale = Vector3.zero;
        leftDirIcon.VfxIconRenderer.transform.localScale = Vector3.zero;
        string lean = GetLean(currentDanceStep);
        if (lean == "R") rightDirIcon.animator.PlaySequence("Pulse");
        else leftDirIcon.animator.PlaySequence("Pulse");
        danceIcon.animator.PlaySequence("Pulse");
    }

    
    #endregion

    #region Helpers
    private static string GetLean(DanceStep step) => step == DanceStep.None ? "" : step.ToString().Substring(0, 1);
    private static string GetOrientation(DanceStep step) => step == DanceStep.None ? "" : step.ToString().Substring(2);
    
    private Sprite ResolveSprite(DanceIcon.SchemesSpritesControls scheme, string orientation, bool pressed)
    {
        DanceIcon.SchemesSpritesControls.ControlButtons button = string.IsNullOrEmpty(orientation) ? (scheme.buttons.Count > 0 ? scheme.buttons[0] : null) : scheme.buttons.Find(x => x.buttonName == orientation);
        if (button == null) return scheme.defaultSprite;
        return pressed ? button.pressed : button.active;
    }
    #endregion
    #endregion

    
}

