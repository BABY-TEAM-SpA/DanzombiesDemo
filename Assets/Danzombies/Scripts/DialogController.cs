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
    public int currentDialogText = 0;
    public DialogDataSO dialogData;
    public UnityEvent OnDialogEndEvent;
}

public class DialogController : MonoBehaviour
{
    [SerializeField] private bool activeOnStart = false;
    public bool animateWriting = false;
    private Coroutine currentWrittingRoutine;
    [SerializeField] private GameObject Container;
    [SerializeField] private Image profileImage;
    [SerializeField] private TMP_Text textContainer;
    [SerializeField] private GameObject pin;
    public int currentScriptSequence { get; private set; } = 0;
    public List<DialogSequence> dialogScripts = new List<DialogSequence>();
    
    // Sequence variables
    

    private void OnDisable()
    {
        if (currentWrittingRoutine != null) StopCoroutine(currentWrittingRoutine);
    }

    private void Start()
    {
        if(activeOnStart) ActivateDialogScript(0);
    }

    public void ActivateDialogScript(int scriptNumber = -1)
    {
        if (scriptNumber >=0)
        {
            currentScriptSequence = scriptNumber;
        }
        int currentDialog = dialogScripts[currentScriptSequence].currentDialogText;
        profileImage.sprite = dialogScripts[currentScriptSequence].dialogData.dialogs[currentDialog].profile;
        Container.SetActive(true);
        
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
        int currentDialog = dialogScripts[currentScriptSequence].currentDialogText;
        DialogText dialogText = dialogScripts[currentScriptSequence].dialogData.dialogs[currentDialog].texts.FirstOrDefault(x => x.language == GameManager.language);
        textContainer.text = (dialogText!=null)?dialogText.text:"";
        currentWrittingRoutine = null;
        pin.gameObject.SetActive(true);
    }

    private void ContinueWritting()
    {
        Debug.Log("Continue writting");
        int value =dialogScripts[currentScriptSequence].currentDialogText+1;
        if (value >= dialogScripts[currentScriptSequence].dialogData.dialogs.Count)
        {
            dialogScripts[currentScriptSequence].OnDialogEndEvent?.Invoke();
        }
        else
        {
            dialogScripts[currentScriptSequence].currentDialogText=value;
            ActivateDialogScript();
        }
    }


    /*private IEnumerator Writting()
    {

    }*/
}
