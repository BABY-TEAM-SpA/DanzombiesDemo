using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Interaction
{
    
    [TagField] public string tag;
    [SerializeField] UnityEvent OnEnter;
    int timesIntended; // <- Veces que el Player ha interactuado/entrado en el área de reacció
    [Tooltip("Veces necesarias para que se ejecute el evento")]
    [Range(1, 10)] public int timesToReact = 0;
    [SerializeField] UnityEvent OnInteraction;
    [SerializeField] UnityEvent OnComplete;
    [SerializeField] UnityEvent OnExit;

    public void React()
    {
        OnEnter.Invoke();
    }
    
    public void Interact()
    {
        timesIntended++;
        OnInteraction.Invoke();
        if (timesIntended >= timesToReact)
        {
            InteractionComplete();
        }
    }
    public void InteractionComplete()
    {
        OnComplete?.Invoke();
    }
    
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
    

    #region [METHODS]
    
    #region IResettable
    
    public void CaptureState()
    {
        
    }

    public void ResetState()
    {
        
    }
    #endregion
    
   
    
    
    
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
    
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Interactuable interactuable))
        {
            //Debug.Log(collision.transform.tag+" entered");
            interactuable.SetInteractive(this);
            HandleReaction(collision.transform.tag);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
        if (collision.TryGetComponent(out Interactuable interactuable))
        {
            //Debug.Log(collision.transform.tag+" leave");
            interactuable.ClearInteractive(this);
            HandleLeave(collision.transform.tag);
            
        }
    }
    #endregion
}