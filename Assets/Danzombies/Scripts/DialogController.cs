using System;
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
        int currentDialog = currentDialogSequence.currentDialogText;
        profileImage.sprite = currentDialogSequence.dialogData.dialogs[currentDialog].profile;
        

        DialogText dialogText = currentDialogSequence.dialogData.dialogs[currentDialog].texts.FirstOrDefault(x => x.language == GameManager.language);
        fullTextTarget = (dialogText != null) ? dialogText.text : "";
        

        if (animateWriting && !string.IsNullOrEmpty(fullTextTarget))
        {
            isWriting = true;
            currentCharacterCount = 0;
            letterTimer = 0f;
            textContainer.text = "";
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
        currentDialogSequence.OnDialogEndEvent?.Invoke();
        base.EndEvent();
    }
}
