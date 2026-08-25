using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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

        public UnityEvent OnStateEntered;
        public UnityEvent OnStateExited;
    }
    #endregion

    #region [UNITY]
    private void Awake()
    {
        states = states.OrderByDescending(s => s.count).ToArray();
        Reset();
    }
    #endregion

    #region [METHODS]
    public void Increase(int value)
    {
        ComboState prevState = State;

        count = Mathf.Max(count + value, 0);

        if (prevState != State) // <- Se compara con el getter del State, por eso prevState puede diferir de State
        {
            GetComboState(prevState)?.OnStateEntered?.Invoke();
            GetComboState(State)?.OnStateExited?.Invoke();
        }

        // <- Aquí iría el llamado a un ComboUIController o similar, mismo patrón que con PlayerFlowController y DanceBarController
        Debug.Log($"[PlayerComboController] Combo = {count}");
    }

    public void Reset() => Increase(-count);

    #region Helpers
    private PlayerComboState GetComboState(ComboState state)
        => states.FirstOrDefault(s => state == s.state);
    #endregion
    #endregion
}
