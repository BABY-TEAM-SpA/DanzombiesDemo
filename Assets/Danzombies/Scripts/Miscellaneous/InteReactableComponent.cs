using FMODUnity;
using System;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Interaction
{
    [TagField] public string tag;
    private int timesIntended; // <- Veces que el Player ha interactuado/entrado en el área de reacción

    [Tooltip("Veces necesarias para que se ejecute el evento")]
    [Range(1, 10)] public int timesToReact = 1;

    [SerializeField] private UnityEvent OnEnter;
    [SerializeField] private UnityEvent OnInteraction;
    [SerializeField] private UnityEvent OnComplete;
    [SerializeField] private UnityEvent OnExit;

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
        OnExit?.Invoke();
        timesIntended = 0;
    }
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
    private struct InteReactableComponentState
    {

    }

    public void CaptureState()
    {

    }

    public void ResetState()
    {

    }
    #endregion
}