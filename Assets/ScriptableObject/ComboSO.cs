using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "newComboData", menuName = "Danzombies/ComboSO")]
public class ComboSO : ScriptableObject
{
    public List<DanceStep> sequence = new List<DanceStep>();
}