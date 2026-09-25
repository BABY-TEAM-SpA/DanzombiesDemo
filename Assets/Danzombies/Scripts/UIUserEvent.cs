using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIUserEvent : MonoBehaviour
{
    private Button submitButton; 
    [SerializeField] private InputActionReference submitAction;
    protected bool isActive = false;
    
    private void OnEnable()
    {
        submitAction.action.performed += OnSubmitPressed;
        submitAction.action.Enable();
        
    }

    private void OnDisable()
    {
        submitAction.action.performed -= OnSubmitPressed;
        submitAction.action.Disable();
    }

    private void Start()
    {
        TryGetComponent<Button>(out submitButton);
    }

    private void OnSubmitPressed(InputAction.CallbackContext context)
    {
        if(isActive) HandleInputTrigger();
    }

    protected virtual void HandleInputTrigger()
    {
        EndEvent();
    }

    public virtual void ActivateEvent()
    {
        isActive = true;
    }

    protected virtual void EndEvent()
    {
        isActive = false;
        submitButton?.onClick?.Invoke();
    }
}
