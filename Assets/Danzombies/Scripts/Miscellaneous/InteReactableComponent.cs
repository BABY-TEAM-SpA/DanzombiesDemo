using System;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Interaction
{
    #region [VARIABLES]
    [TagField] public string tag;

    [Tooltip("Veces necesarias para que se ejecute el evento")]
    [Min(1)] public int timesToReact = 1;

    private int timesIntended; // <- Veces que el Player ha interactuado/entrado en el área de reacción
    [HideInInspector] public bool completed;
    private InteractionState _state;

    [SerializeField] private UnityEvent OnEnter;
    [SerializeField] private UnityEvent OnInteraction;
    [SerializeField] private UnityEvent OnComplete;
    [SerializeField] private UnityEvent OnExit;

    [Header("Reset")]
    public UnityEvent OnReset;
    #endregion

    #region [METHODS]
    public void React() => OnEnter.Invoke();
    
    public void Interact()
    {
        timesIntended++;
        OnInteraction.Invoke();

        if (timesIntended >= timesToReact)
            InteractionComplete();
    }

    public void InteractionComplete() => OnComplete?.Invoke();
    
    public void Leave()
    {
        timesIntended = 0;
        OnExit?.Invoke();
    }
    #endregion

    #region IResettable
    private struct InteractionState
    {
        public bool completed;
    }

    public void Capture()
    {
        _state = new InteractionState
        {
            completed = completed,
        };
    }

    public void Reset()
    {
        if (completed && !_state.completed)
        {
            timesIntended = 0;
            completed = false;

            OnReset?.Invoke();
        }        
    }
    #endregion
}

[RequireComponent(typeof(Collider2D))]
public class InteReactableComponent : MonoBehaviour, IResettable
{
    #region [VARIABLES]
    [SerializeField] private Interaction[] interactions;
    #endregion

    #region [UNITY]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.transform.tag + " entered");

        if (collision.TryGetComponent(out Interactuable interactuable))
            interactuable.SetInteractive(this);

        HandleReaction(collision.transform.tag);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log(collision.transform.tag + " leave");

        if (collision.TryGetComponent(out Interactuable interactuable))
            interactuable.ClearInteractive(this);

        HandleLeave(collision.transform.tag);
    }
    #endregion

    #region [METHODS]
    public void HandleReaction(string type)
    {
        Interaction e = interactions.FirstOrDefault(interaction => type == interaction.tag);
        e?.React();
    }

    public void HandleInteraction(string type)
    {
        Interaction e = interactions.FirstOrDefault(interaction => type == interaction.tag);
        e?.Interact();
    }

    public void HandleLeave(string type)
    {
        Interaction e = interactions.FirstOrDefault(interaction => type == interaction.tag);
        e?.Leave();
    }
    #endregion

    #region IResettable
    public void MarkAsCompleted(string type)
    {
        Interaction e = interactions.FirstOrDefault(interaction => type == interaction.tag);
        if (e != null)
            e.completed = true;
    }

    public void CaptureState()
    {
        foreach (Interaction e in interactions)
            e.Capture();
    }

    public void ResetState()
    {
        foreach (Interaction e in interactions)
            e.Reset();
    }
    #endregion
}