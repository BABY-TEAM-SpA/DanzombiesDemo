using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DamageMode
{
    None,
    ModificaFlow,
    ModificaFlowYDaña
}

public interface IDanceInputZone
{
    public void SetPlayerInput(DanceStep step, out BeatReciever.BeatFeedback bf);
    public DamageMode GetDamageMode();
}

public class DanceZone : Dancer, IDanceInputZone
{
    public SpriteRenderer Renderer;
    [Header("Dance Zone Settings")]
    [SerializeField] private List<Dancer> dancers = new List<Dancer>();
    public DanceEventManager listeners = new DanceEventManager();
    
    [Header("Players")] 
    protected bool PlayerHasDanced=false;
    public PlayerManager playersInside{private set; get;}

    [SerializeField]
    private DamageMode damageMode;
    
    public DamageMode GetDamageMode()
    {
        return damageMode;
    }

    public void Start()
    {
        if(dancers.Count > 0) SetZone();
    }

    public void SetZone()
    {
        foreach (ZombieDanceBrain dancer in dancers) listeners.AddListener(dancer);
    }
    
    public override void OnDanceStepAction(DanceStep step)
    {
        listeners.InvokeDance(step);
        onDance?.Invoke(step);
    }

    private void OnDisable()
    {
        listeners.RemoveAllListeners();
    }

    private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
                PlayerEnter(player);
        }
    public virtual void PlayerEnter(PlayerManager player)
    {
        player.AddTargetPuzzle(this);
        playersInside = player;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
            PlayerLeave(player);
    }
    public virtual void PlayerLeave(PlayerManager player)
    {
        playersInside = null;
        player.RemoveTargetPuzzle(this);
    }

    public override void OnPrepareStepAction(DanceStep step)
    {
        PlayerHasDanced = false;
        Renderer.enabled = true;
        base.OnPrepareStepAction(step);
    }

    public override void OnReleaseStepAction(DanceStep step)
    {
        if (playersInside!= null &&!PlayerHasDanced && step != DanceStep.None)
        {
            Debug.Log("didntDance");
            playersInside?.ApplyDanceFeedback(BeatReciever.BeatFeedback.Bad);
        }
        Renderer.enabled = false;
        base.OnReleaseStepAction(step);
    }
    

    public void SetPlayerInput(DanceStep step, out BeatReciever.BeatFeedback bf)
    {
        Debug.Log(step.ToString()+currentDanceStep.ToString());
        
        bf = BeatReciever.BeatFeedback.Bad;
        if (PlayerHasDanced) return;
        else
        {
            PlayerHasDanced = true;
            bool isCorrect = step == currentDanceStep;
            bf = isCorrect ? BeatManager.Instance.EvaluateInput() : BeatReciever.BeatFeedback.Bad;
            React(bf==BeatReciever.BeatFeedback.Bad?ExpressionType.Angry:ExpressionType.Normal);
        }
        
    }
    public void React(ExpressionType exp)
    {
        foreach (ZombieDanceBrain dancer in dancers)
            dancer.React(exp);
        
    }
}