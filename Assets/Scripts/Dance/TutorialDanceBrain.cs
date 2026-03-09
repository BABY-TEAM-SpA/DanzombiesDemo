using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialDanceBrain : MonoBehaviour
{
    
    [SerializeField] PlayerInput _playerInput;
    private string currentScheme;
    
    [SerializeField] private Canvas danceCanvas;
    
    [SerializeField] private DanceIcon danceIcon;
    private DanceIcon.SchemesSpritesControls currentDanceScheme = new DanceIcon.SchemesSpritesControls();
    [SerializeField] private DanceIcon leftDirIcon;
    private DanceIcon.SchemesSpritesControls currentLeftScheme = new DanceIcon.SchemesSpritesControls();
    [SerializeField] private DanceIcon rightDirIcon;
    private DanceIcon.SchemesSpritesControls currentRightScheme = new DanceIcon.SchemesSpritesControls();
    
    [SerializeField, Range(0f,1f)] private float  unactiveAlpha;

    private DanceStep futureDanceStep = DanceStep.None;
    

    public void Start()
    {
        PrepareUI();
    }

    public void Connect(RhythmPuzzle puzzle)
    {
        puzzle.OnPrepareStep += OnPrepareStepAction;
        puzzle.OnDanceStep += OnDanceStepAction;
        puzzle.OnReleaseStep += OnReleaseStepAction;
    }
    public void Disconnect(RhythmPuzzle puzzle)
    {
        puzzle.OnPrepareStep -= OnPrepareStepAction;
        puzzle.OnDanceStep -= OnDanceStepAction;
        puzzle.OnReleaseStep -= OnReleaseStepAction;
    }

    private void OnPrepareStepAction(DanceStep step)
    {
        if (step != DanceStep.None && danceCanvas.isActiveAndEnabled)
        {
            string view = step.ToString()[0].ToString();
            string orientation = step.ToString().Remove(0,2);
            danceIcon.iconRenderer.sprite = currentDanceScheme.buttons.Find(x => x.buttonName == orientation).active;
            
        }
    }
    private void OnDanceStepAction(DanceStep step)
    {
        if (step != DanceStep.None&& danceCanvas.isActiveAndEnabled)
        {
            string view = step.ToString()[0].ToString();
            string orientation = step.ToString().Remove(0,2);
            
            danceIcon.iconRenderer.sprite = currentDanceScheme.buttons.Find(x => x.buttonName == orientation).pressed;
            danceIcon.animator.PlaySequence("Pulse");
            
            if (view == "R")
            {
                rightDirIcon.iconRenderer.color = Color.white;
                rightDirIcon.iconRenderer.sprite = currentRightScheme.buttons[0].pressed;
                rightDirIcon.animator.PlaySequence("Pulse");
            }
            else
            {
                leftDirIcon.iconRenderer.color = Color.white;
                leftDirIcon.iconRenderer.sprite = currentLeftScheme.buttons[0].pressed;
                leftDirIcon.animator.PlaySequence("Pulse");
            };
        }
    }
    private void OnReleaseStepAction(DanceStep step, DanceStep futureStep)
    {
        
        if (step != DanceStep.None&& danceCanvas.isActiveAndEnabled)
        {
            string view = step.ToString()[0].ToString();
            string orientation = step.ToString().Remove(0,2);
            danceIcon.iconRenderer.sprite = currentDanceScheme.buttons.Find(x => x.buttonName == orientation).active;
            if (view == "R")
            {
                rightDirIcon.iconRenderer.sprite = currentRightScheme.buttons[0].active;
            }
            else
            {
                leftDirIcon.iconRenderer.sprite = currentLeftScheme.buttons[0].active;
            };
        }
        futureDanceStep = futureStep;
        PrepareUI();
    }

    public void PrepareUI()
    {
        danceIcon.iconRenderer.color = new Color(1, 1, 1, unactiveAlpha);
        rightDirIcon.iconRenderer.color = new Color(1, 1, 1, unactiveAlpha);
        leftDirIcon.iconRenderer.color = new Color(1, 1, 1, unactiveAlpha);
        string view = futureDanceStep.ToString()[0].ToString();
        string orientation = futureDanceStep.ToString().Remove(0,2);
        
        //Orientacion
        currentScheme = _playerInput.currentControlScheme;
        currentDanceScheme = danceIcon.schemes.FirstOrDefault(x=>x.schemeName==currentScheme);
        DanceIcon.SchemesSpritesControls.ControlButtons button = currentDanceScheme.buttons.FirstOrDefault(x => x.buttonName == orientation);
        danceIcon.iconRenderer.sprite = (button!=null)?button.active:currentDanceScheme.defaultSprite;
        danceIcon.iconRenderer.SetNativeSize();
        danceIcon.iconFXRenderer.sprite = currentDanceScheme.spriteFX;
        danceIcon.iconFXRenderer.SetNativeSize();
        
        // Costados
        currentLeftScheme = leftDirIcon.schemes.FirstOrDefault(x=>x.schemeName==currentScheme);
        leftDirIcon.iconRenderer.sprite = currentLeftScheme.buttons[0].active;
        leftDirIcon.iconRenderer.SetNativeSize();
        leftDirIcon.iconFXRenderer.sprite = currentLeftScheme.spriteFX;
        leftDirIcon.iconFXRenderer.SetNativeSize();
        currentRightScheme = rightDirIcon.schemes.FirstOrDefault(x=>x.schemeName==currentScheme);
        rightDirIcon.iconRenderer.sprite = currentRightScheme.buttons[0].active;
        rightDirIcon.iconRenderer.SetNativeSize();
        rightDirIcon.iconFXRenderer.sprite = currentRightScheme.spriteFX;
        rightDirIcon.iconFXRenderer.SetNativeSize();

        if (futureDanceStep != DanceStep.None)
        {
            danceIcon.iconRenderer.color = Color.white;
            if(view =="R") rightDirIcon.iconRenderer.color = Color.white;
            else leftDirIcon.iconRenderer.color = Color.white;
        }
        
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
    
    ///////////////////////////////////////////////////
    
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
