using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class DialogSequence
{
    [HideInInspector] public int currentDialogText = 0;
    public float timeToAutoContinue;
    public DialogDataSO dialogData;
    public UnityEvent OnDialogEndEvent;
}

public class DialogController : UIUserEvent
{
    private bool isWriting = false;
    public bool animateWriting = false;
    [SerializeField] private float timePerLetter = 0.03f;

    [SerializeField] private Button dialogRender;
    [SerializeField] private Image profileImage;
    [SerializeField] private TMP_Text textContainer;
    [SerializeField] private GameObject pin;
    
    private List<TalkingPerson> talkingPersons = new List<TalkingPerson>();
    DanceBrain talkingBrain;
    
    private DialogSequence currentDialogSequence;
    private float currentTimer;
    
    private string fullTextTarget = "";
    private int currentCharacterCount = 0;
    private float letterTimer = 0f;
    
    public static DialogController Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) return;
        Instance = this;
    }

    private void Update()
    {
        if(!isActive) return;
        if (isWriting)
        {
            letterTimer += Time.deltaTime;
            if (letterTimer >= timePerLetter)
            {
                letterTimer = 0f;
                currentCharacterCount++;
                
                textContainer.text = fullTextTarget.Substring(0, currentCharacterCount);

                if (currentCharacterCount >= fullTextTarget.Length) OnWrittingComplete();
            }
        }
        else
        {
            if (currentTimer > 0 && currentDialogSequence.timeToAutoContinue > 0)
            {
                currentTimer -= Time.deltaTime;
                if (currentTimer <= 0) ContinueWritting();
            }
        }
    }
    
    protected override void HandleInputTrigger()
    {
        if (isWriting) OnWrittingComplete();
        else ContinueWritting();
    }

    public void SetTalkingPersons(List<TalkingPerson> persons)
    {
        talkingPersons = persons;
    }

    public void PlayDialog(DialogSequence dialog)
    {
        currentDialogSequence = dialog;
        currentDialogSequence.currentDialogText = 0;
        ActivateEvent();
    }

    public override void ActivateEvent()
    {
        base.ActivateEvent();
        dialogRender.gameObject.SetActive(true);
        pin.gameObject.SetActive(false);
        currentTimer = 0f; 
        Dialog dialog = currentDialogSequence.dialogData.dialogs[currentDialogSequence.currentDialogText];
        profileImage.sprite = dialog.profile;
        string characterName = dialog.Character;
        DialogExpression expression = dialog.expression;
        
        
        
        DialogText dialogText = dialog.texts.FirstOrDefault(x => x.language == GameManager.Instance.Language);
        fullTextTarget = (dialogText != null) ? dialogText.text : "";
        
        if (animateWriting && !string.IsNullOrEmpty(fullTextTarget))
        {
            isWriting = true;
            currentCharacterCount = 0;
            letterTimer = 0f;
            textContainer.text = "";
            TalkingPerson person = talkingPersons.FirstOrDefault(x=> x.name == characterName);
            if(person != null)
            {
                talkingBrain = person.brain;
                talkingBrain.OnTalkingStart(expression);
            }
        }
        else
        {
            textContainer.text = fullTextTarget;
            OnWrittingComplete();
        }
    }

    private void OnWrittingComplete()
    {
        isWriting = false;
        talkingBrain?.OnTalkingEnd();
        talkingBrain = null;
        textContainer.text = fullTextTarget;
        currentTimer = currentDialogSequence.timeToAutoContinue;
        pin.gameObject.SetActive(true);
    }

    public void ContinueWritting()
    {
        int value = currentDialogSequence.currentDialogText + 1;
        if (value < currentDialogSequence.dialogData.dialogs.Count)
        {
            currentDialogSequence.currentDialogText = value;
            ActivateEvent();
        }
        else EndEvent();
    }

    protected override void EndEvent()
    {
        
        dialogRender.gameObject.SetActive(false);
        base.EndEvent();
        currentDialogSequence.OnDialogEndEvent?.Invoke();
        
    }
}
