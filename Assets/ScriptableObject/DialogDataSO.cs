using UnityEngine;
using System;
using System.Collections.Generic;

public enum Languages
{
    Spanish,
    English
}

[Serializable]
public class Dialog
{
    public Sprite profile;
    public string Character;
    public List<DialogText> texts = new List<DialogText>();
}

[Serializable]
public class DialogText
{
    public Languages language;
    public string text;
}

[CreateAssetMenu(fileName = "newDialogData", menuName = "Danzombies/DialogDataSO")]
public class DialogDataSO : ScriptableObject
{
    public List<Dialog> dialogs = new List<Dialog>();
}
