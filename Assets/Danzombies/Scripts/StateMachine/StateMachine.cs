using System;
using System.Collections.Generic;
using UnityEngine;



public class StateMachine : MonoBehaviour
{
    public bool isActive;
    public State CurrentState { get; private set; }
    
    public void ChangeState(State newState)
    {
        if (newState == null || newState == CurrentState)
            return;
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    private void Update()
    {
        if (!isActive) return;
        if (CurrentState == null) return;
        
        CurrentState.Update();
        State nextState = CurrentState.CheckTransitions();
        if (nextState != null) ChangeState(nextState);
    }
}