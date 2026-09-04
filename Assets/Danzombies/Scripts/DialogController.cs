using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class DialogSequence
{
    [HideInInspector] public int currentDialogText = 0;
    public float timeToAutoContinue;
    public DialogDataSO dialogData;
    public UnityEvent OnDialogEndEvent;
}

public class DialogController : MonoBehaviour
{
    public bool animateWriting = false;
    private Coroutine currentWrittingRoutine;
    [SerializeField] private GameObject dialogRender;
    [SerializeField] private Image profileImage;
    [SerializeField] private TMP_Text textContainer;
    [SerializeField] private GameObject pin;
    
    private DialogSequence currentDialogSequence;
    private float currentTimer;
    
    public static DialogController Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
            return;
        Instance = this;
    }

    private void OnDisable()
    {
        if (currentWrittingRoutine != null) StopCoroutine(currentWrittingRoutine);
    }

    private void Update()
    {
        if (currentTimer > 0 && currentDialogSequence.timeToAutoContinue > 0)
        {
            currentTimer -= Time.deltaTime;
            if (currentTimer <= 0)
            {
                ContinueWritting();
            }
        }
    }

    public void PlayDialog(DialogSequence dialog)
    {
        currentDialogSequence =  dialog;
        ActivateDialogScript();
    }
    

    public void ActivateDialogScript()
    {
        currentTimer = currentDialogSequence.timeToAutoContinue;
        int currentDialog = currentDialogSequence.currentDialogText;
        profileImage.sprite = currentDialogSequence.dialogData.dialogs[currentDialog].profile;
        dialogRender.SetActive(true);
        if (animateWriting)
        {
            
        }
        else
        {
            OnWrittingComplete();
        }
    }

    private void OnWrittingComplete()
    {
        int currentDialog = currentDialogSequence.currentDialogText;
        DialogText dialogText = currentDialogSequence.dialogData.dialogs[currentDialog].texts.FirstOrDefault(x => x.language == GameManager.language);
        textContainer.text = (dialogText!=null)?dialogText.text:"";
        currentWrittingRoutine = null;
        pin.gameObject.SetActive(true);
    }

    public void ContinueWritting()
    {
        //Debug.Log("Continue writting");
        int value =currentDialogSequence.currentDialogText+1;
        if (value >= currentDialogSequence.dialogData.dialogs.Count)
        {
            dialogRender.SetActive(false);
            currentDialogSequence.OnDialogEndEvent?.Invoke();
        }
        else
        {
            currentDialogSequence.currentDialogText=value;
            ActivateDialogScript();
        }
    }
    
    /*private IEnumerator Writting()
    {

    }*/
}
