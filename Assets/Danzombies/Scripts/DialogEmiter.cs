using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TalkingPerson
{
    public string name;
    public DanceBrain brain;
}



public class DialogEmiter : MonoBehaviour
{

    public List<TalkingPerson> talkingPersons = new List<TalkingPerson>();
    public bool playOnStart = false;
    public List<DialogSequence> dialogScripts = new List<DialogSequence>();
    private int currentSequenceIndex = 0;
    
    

    private void Start()
    {
        if (playOnStart)
        {
            Invoke("PlayInitialDialog",0.5f);
        }
    }

    private void PlayInitialDialog()
    {
        PlayDialogByIndex(0);
    }
    public void PlayDialogByIndex(int index=0)
    {
        DialogController.Instance.SetTalkingPersons(talkingPersons);
        if (dialogScripts[index] != null)
        {
            currentSequenceIndex = index;
            DialogController.Instance.PlayDialog(dialogScripts[index]);
        }
    }
}
