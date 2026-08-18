using System;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Interaction
{
    private enum InteractionType
    {
        OnEnter,
        OnExit,
        OnInteraction,
    }
    #region [VARIABLES]
    [TagField] public string target;
    [HideInInspector] public bool completed;
    private int timesIntended; // <- Veces que el Player ha interactuado/entrado en el área de reacción

    [SerializeField] private UnityEvent OnEnter;
    [SerializeField] private UnityEvent OnInteraction;

    [Tooltip("Veces necesarias para que se ejecute el evento.")]
    [Min(0)] public int timesToComplete = 1;
    [SerializeField] private UnityEvent OnComplete;
    [SerializeField] private UnityEvent OnExit;
    #endregion

    #region [METHODS]
    public void React() => OnEnter.Invoke();
    
    public void Interact()
    {
        if (completed)
            return;

        timesIntended++;
        if (timesIntended == timesToComplete)
            InteractionComplete();
        else OnInteraction.Invoke();
    }

    public void InteractionComplete() => OnComplete?.Invoke();
    
    public void Leave()
    {
        timesIntended = 0;
        OnExit?.Invoke();
    }
    #endregion
}

[RequireComponent(typeof(Collider2D))]
public class InteReactableComponent : MonoBehaviour
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
    #region Handlers
    public void HandleReaction(string type)
    {
        if (GetInteraction(type, out Interaction interaction))
            interaction.React();
    }

    public void HandleInteraction(string type)
    {
        if (GetInteraction(type, out Interaction interaction))
            interaction.Interact();
    }

    public void HandleLeave(string type)
    {
        if (GetInteraction(type, out Interaction interaction))
            interaction.Leave();
    }
    #endregion

    #region Completeness
    public void MarkAsCompleted(string type)
    {
        if (GetInteraction(type, out Interaction interaction))
            interaction.completed = true;
    }

    public void MarkAsUncompleted(string type)
    {
        if (GetInteraction(type, out Interaction interaction))
            interaction.completed = false;
    }
    #endregion

    #region Helpers
    private bool GetInteraction(string type, out Interaction interaction)
    {
        interaction = interactions.FirstOrDefault(e => type == e.target);
        return interaction != null;
    }
    #endregion
    #endregion
}