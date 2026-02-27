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
    
    [SerializeField] private DanceIcon danceIcon;
    [SerializeField] private DanceIcon leftDirIcon;
    [SerializeField] private DanceIcon rightDirIcon;

    private DanceIcon currentDirectionIcon;

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
        PrepareUI();
        if (step != DanceStep.None)
        {
            string view = step.ToString()[0].ToString();
            string orientation = step.ToString().Remove(0,2);
            if(view == "R")   Debug.Log("Play R Settup");
            else Debug.Log("Play L Settup");
            
        }
    }
    private void OnDanceStepAction(DanceStep step)
    {
        if (step != DanceStep.None)
        {
            string view = step.ToString()[0].ToString();
            string orientation = step.ToString().Remove(0,2);
            
            if(view == "R") Debug.Log("Play R Animation");
            else Debug.Log("Play L Animation");//rightDirIcon?.animator.Play();
            //playerAnimCtrl?.animator.SetBool("DanceStep"+orientation[0],true);
            //playerAnimCtrl?.animator.SetTrigger("Dance");
        }
    }
    private void OnReleaseStepAction(DanceStep step)
    {
        
        if (step != DanceStep.None)
        {
            string view = step.ToString()[0].ToString();
            string orientation = step.ToString().Remove(0,2);
        }
    }

    public void PrepareUI()
    {
        currentScheme = _playerInput.currentControlScheme;
        DanceIcon.SchemesSpritesControls schemeDance = danceIcon.schemes.FirstOrDefault(x=>x.schemeName==currentScheme);
        danceIcon.iconRenderer.sprite = schemeDance.defaultSprite;
        danceIcon.iconRenderer.SetNativeSize();
        danceIcon.iconFXRenderer.sprite = schemeDance.spriteFX;
        danceIcon.iconFXRenderer.SetNativeSize();
        DanceIcon.SchemesSpritesControls schemeLeft = leftDirIcon.schemes.FirstOrDefault(x=>x.schemeName==currentScheme);
        leftDirIcon.iconRenderer.sprite = schemeLeft.buttons[0].active;
        leftDirIcon.iconRenderer.SetNativeSize();
        leftDirIcon.iconFXRenderer.sprite = schemeLeft.spriteFX;
        leftDirIcon.iconFXRenderer.SetNativeSize();
        DanceIcon.SchemesSpritesControls schemeRight = rightDirIcon.schemes.FirstOrDefault(x=>x.schemeName==currentScheme);
        rightDirIcon.iconRenderer.sprite = schemeRight.buttons[0].active;
        rightDirIcon.iconRenderer.SetNativeSize();
        rightDirIcon.iconFXRenderer.sprite = schemeRight.spriteFX;
        rightDirIcon.iconFXRenderer.SetNativeSize();
    }
    
    ////////////////////////////////////////////////////summary>
    
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
