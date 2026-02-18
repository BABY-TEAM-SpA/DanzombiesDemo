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
    public int currentSequence { get; private set; } = 0;
    public List<DialogSequence> dialogScripts = new List<DialogSequence>();
    
    // Sequence variables
    

    private void OnDisable()
    {
        if (currentWrittingRoutine != null) StopCoroutine(currentWrittingRoutine);
    }

    public void Start()
    {
        if(activeOnStart) WriteDialog(0);
    }

    public void WriteDialog(int dialogNumber = -1)
    {
        if (dialogNumber != -1)
        {
            currentSequence = dialogNumber;
        }
        int currentDialog = dialogScripts[currentSequence].currentDialogText;
        profileImage.sprite = dialogScripts[currentSequence].dialogData.dialogs[currentDialog].profile;
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
        int currentDialog = dialogScripts[currentSequence].currentDialogText;
        DialogText dialogText = dialogScripts[currentSequence].dialogData.dialogs[currentDialog].texts.FirstOrDefault(x => x.language == GameManager.Instance.language);
        textContainer.text = (dialogText!=null)?dialogText.text:"";
        currentWrittingRoutine = null;
        pin.gameObject.SetActive(true);
    }

    public void ContinueWritting()
    {
        int value =dialogScripts[currentSequence].currentDialogText+1;
        if (value >= dialogScripts[currentSequence].dialogData.dialogs.Count)
        {
            dialogScripts[currentSequence].OnDialogEndEvent?.Invoke();
        }
        else
        {
            dialogScripts[currentSequence].currentDialogText=value;
            WriteDialog();
        }
    }


    /*private IEnumerator Writting()
    {

    }*/
}
