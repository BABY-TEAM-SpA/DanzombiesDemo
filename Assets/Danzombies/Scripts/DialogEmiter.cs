using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogEmiter : MonoBehaviour
{

    public bool playOnStart = false;
    public List<DialogSequence> dialogScripts = new List<DialogSequence>();
    private int currentSequenceIndex = 0;

    private void Start()
    {
        if (playOnStart)
        {
            PlayDialogByIndex(0);
        }
    }

    public void PlayDialogByIndex(int index)
    {
        if (dialogScripts[index] != null)
        {
            currentSequenceIndex = index;
            DialogController.Instance.PlayDialog(dialogScripts[index]);
        }
    }
}
