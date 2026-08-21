using System;
using System.Linq;
using UnityEngine;

public enum ComboState { S, A, B, C, D };

public class PlayerComboController : MonoBehaviour
{
    #region [VARIABLES]
    public ComboState State => states.FirstOrDefault(x => Count >= x.count)?.state ?? ComboState.D;

    public int Count => count;
    private int count;

    [SerializeField] private PlayerComboState[] states;
    [Serializable]
    private class PlayerComboState
    {
        public ComboState state;
        [Tooltip("A partir de este número, el Combo entra a este estado.")]
        [Min(0)] public int count;
    }
    #endregion

    #region [UNITY]
    private void Start() => states = states.OrderByDescending(s => s.count).ToArray();
    #endregion

    #region [METHODS]
    public void Increase() => count = Mathf.Max(count + 1, 0);
    public void Decrease() => count = Mathf.Max(count - 1, 0);

    public void Reset() => count = 0;
    #endregion
}
