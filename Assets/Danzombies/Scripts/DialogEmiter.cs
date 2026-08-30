using System.Collections.Generic;
using UnityEngine;

public class DialogEmiter : MonoBehaviour
{
    public List<DialogSequence> dialogScripts = new List<DialogSequence>();
    private int currentSequenceIndex = 0;

    public void PlayDialogByIndex(int index)
    {
        if (dialogScripts[index] != null)
        {
            currentSequenceIndex = index;
            DialogController.Instance.PlayDialog(dialogScripts[index]);
        }
    }
}
