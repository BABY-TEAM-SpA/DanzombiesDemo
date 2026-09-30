using UnityEngine;
using System;
using System.Collections.Generic;

public enum Languages
{
    Spanish,
    English
}

public enum DialogExpression
{
    Normal,
    Happy,
    Fear,
    Angry,
    Sad
}

[Serializable]
public class DialogText
{
    public Languages language;
    public string text;
}

[Serializable]
public class Dialog
{
    public Sprite profile;
    public string Character;
    public DialogExpression expression;
    public List<DialogText> texts = new List<DialogText>();
}


[CreateAssetMenu(fileName = "newDialogData", menuName = "Danzombies/DialogDataSO")]
public class DialogDataSO : ScriptableObject
{
    public List<Dialog> dialogs = new List<Dialog>();
}
