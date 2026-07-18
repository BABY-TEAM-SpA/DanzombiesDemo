using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class State
{
    
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();

    private List<Transition> transitions = new(); //segun prioridad de cambio.

    public void AddTransition(State target, Func<bool> condition)
    {
        transitions.Add(new Transition(target, condition));
    }

    public State CheckTransitions()
    {
        foreach (Transition transition in transitions)
        {
            if (transition.Condition())
                return transition.TargetState;
        }

        return null;
    }
}
public class Transition
{
    public State TargetState { get; private set; }

    public Func<bool> Condition { get; private set; }

    public Transition(State target, Func<bool> condition)
    {
        TargetState = target;
        Condition = condition;
    }
}
