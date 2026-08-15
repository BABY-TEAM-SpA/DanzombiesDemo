using System;
using UnityEngine;


public class IdleState : State
{
    public bool getsMotivated;
    public override void Enter()
    {
        
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        
    }
}

public class PatrolState : State
{
    public bool isLooking;

    public override void Enter()
    {
        
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        
    }
}
[Serializable]
public class ChaseState : State
{
    [HideInInspector] public bool playerIsNear;
    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }
}
[Serializable]
public class DancingState : State
{
    
    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }
}
public class StunnedState : State
{
    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }
}

public class EnemyBrain : MonoBehaviour
{
    private StateMachine stateMachine;
    [SerializeField] private RhythmPuzzle puzzle;
    
    public Transform Player { get; private set; }
    public bool SawPlayer { get; private set; }
    public bool Near { get; private set; }
    public bool IsStunned { get; private set; }


    [SerializeField] private IdleState idle =new IdleState();
    [SerializeField] private PatrolState patrol = new PatrolState();
    [SerializeField] private ChaseState chase = new ChaseState();
    [SerializeField] private DancingState dancing = new DancingState();
    [SerializeField] private StunnedState stunned = new StunnedState();


    private void Awake()
    {
        stateMachine = new StateMachine();

        idle.AddTransition(patrol,() => idle.getsMotivated);
        idle.AddTransition(chase,() => SawPlayer);
        
        patrol.AddTransition(idle,() => !patrol.isLooking);
        patrol.AddTransition(chase,() => SawPlayer);
        
        chase.AddTransition(idle,() => !SawPlayer);
        //chase.AddTransition(dancing,() => puzzle.playersInside!=null); Sorry Franco es culpa de la feña
        
        //dancing.AddTransition(chase,() => puzzle.playersInside==null); Sorry Franco es culpa de la feña
        dancing.AddTransition(stunned,() => IsStunned);

        stunned.AddTransition(idle,() => !IsStunned);
        
        stateMachine.ChangeState(idle);
    }


    private void Update()
    {
        UpdateSensors();
    }


    private void UpdateSensors()
    {
        
        SawPlayer = false;
        Near = false;
        
    }
    
    public void Stun()
    {
        IsStunned = true;
    }


    public void ClearStun()
    {
        IsStunned = false;
    }
}
