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

public class DialogController : MonoBehaviour, ISubmitHandler, IPointerClickHandler
{
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
    private bool isWriting = false;

    private bool pendingFocus = false;

    public static DialogController Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) return;
        Instance = this;
    }

    private void Update()
    {
        if (pendingFocus)
        {
            if (EventSystem.current != null && dialogRender != null) EventSystem.current.SetSelectedGameObject(dialogRender.gameObject);
            pendingFocus = false;
        }

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

        if (!isWriting && currentTimer > 0 && currentDialogSequence.timeToAutoContinue > 0)
        {
            currentTimer -= Time.deltaTime;
            if (currentTimer <= 0) ContinueWritting();
        }
    }
    
    public void OnSubmit(BaseEventData eventData)
    {
        HandleInputTrigger();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        HandleInputTrigger();
    }

    private void HandleInputTrigger()
    {
        if (isWriting) OnWrittingComplete();
        else 
        {
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
            ContinueWritting();
        }
    }

    public void PlayDialog(DialogSequence dialog)
    {
        currentDialogSequence = dialog;
        currentDialogSequence.currentDialogText = 0;
        ActivateDialogScript();
    }

    public void ActivateDialogScript()
    {
        pin.gameObject.SetActive(false);
        currentTimer = 0f; 

        int currentDialog = currentDialogSequence.currentDialogText;
        profileImage.sprite = currentDialogSequence.dialogData.dialogs[currentDialog].profile;
        dialogRender.gameObject.SetActive(true);
        pendingFocus = true;

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
        if (value >= currentDialogSequence.dialogData.dialogs.Count)
        {
            dialogRender.gameObject.SetActive(false);
            currentDialogSequence.OnDialogEndEvent?.Invoke();
        }
        else
        {
            currentDialogSequence.currentDialogText = value;
            ActivateDialogScript();
        }
    }
}
