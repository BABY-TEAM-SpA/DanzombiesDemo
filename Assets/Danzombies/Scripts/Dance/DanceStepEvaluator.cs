using System;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class DanceStepEvaluator
{
    #region [VARIABLES]
    private readonly Queue<StepRegist> expected = new();
    private readonly Queue<StepRegist> player = new();

    public event Action<BeatReciever.BeatFeedback> OnFeedback;
    
    #endregion

    #region [METHODS]
    #region Add
    public void AddExpected(DanceStep step, int timeMs)
    {
        expected.Enqueue(new StepRegist(step, timeMs));
        //Debug.Log($"Expected Added...Expected: {expected.Count}, Player: {player.Count}");
    }

    public void AddPlayer(DanceStep step, int timeMs)
    {
        
        player.Enqueue(new StepRegist(step, timeMs));
        //Debug.Log($"Player Added!!!! ...Expected: {expected.Count}, Player: {player.Count}");
    }
    #endregion
    

    #region Manage
    private void ManageQueues(int nowMs)
    {
        if(expected.TryPeek(out StepRegist expectedRegist) && player.TryPeek(out StepRegist playerRegist))
        {
            expectedRegist = expected.Dequeue();
            playerRegist = player.Dequeue();
            
            BeatReciever.BeatFeedback grade = Grade(expectedRegist,playerRegist);
            //Debug.Log($"Resolved{grade}");
            OnFeedback?.Invoke(grade);
        }
        if(expected.TryPeek(out StepRegist regExp))
        {
            if (regExp.IsExpired(nowMs))
            {
                expected.Dequeue();
                OnFeedback?.Invoke(BeatReciever.BeatFeedback.Bad); 
            }
        }
        if(player.TryPeek(out StepRegist regPlayer))
        {
            if(regPlayer.IsExpired(nowMs))
            {
                player.Dequeue();
                OnFeedback?.Invoke(BeatReciever.BeatFeedback.Bad); 
            }
        }
    }
    private BeatReciever.BeatFeedback Grade(StepRegist expectedRegist,StepRegist  playerRegist)
    {
        if(expectedRegist.danceStep != playerRegist.danceStep) return BeatReciever.BeatFeedback.Bad;
        int absDelta = Math.Abs(expectedRegist.stepTimeMs - playerRegist.stepTimeMs);
        BeatManager bm = BeatManager.Instance;
        float max = bm.marginMs;
        float great = max * bm.greatPercentOnMargin;
        float perfect = great * bm.perfectPercentOnMargin;
        if (absDelta <= perfect) return BeatReciever.BeatFeedback.Perfect;
        if (absDelta <= great) return BeatReciever.BeatFeedback.Good;
        return BeatReciever.BeatFeedback.Ok;
    }
    
    /*
    /// <summary>
    /// Busca en la otra lista el StepRegist más cercano en tiempo al registro que expiró y lo marca como 'penalized'
    /// para evitar que se considere como Bad al expirar.
    /// </summary>
    /// <param name="reg">StepRegist de la otra lista, expirado y que fue considerado como Bad.</param>
    /// <param name="other">Lista opuesta y en la que se busca en StepRegist más próximo a expirar.</param>
    private void PenalizeNeighbor(StepRegist reg, List<StepRegist> other)
    {
        StepRegist best = null;
        int bestDelta = int.MaxValue;
        foreach (StepRegist candidate in other)
        {
            if (candidate.penalized) continue;
            int delta = Math.Abs(candidate.stepTimeMs - reg.stepTimeMs);
            if (delta <= Window && delta < bestDelta)
            {
                best = candidate;
                bestDelta = delta;
            }
        }
        if (best != null) best.penalized = true;
    }*/
    #endregion

    #region Helpers
    public void Tick(int nowMs) => ManageQueues(nowMs);

    public void Clear()
    {
        expected.Clear();
        player.Clear();
    }
    #endregion
    #endregion
}

public class StepRegist
{
    public DanceStep danceStep;
    public int stepTimeMs;
    public bool penalized; // <- [Frco] Marcar como verdadero para evitar que la expiración del StepRegist lance un Bad
    private float life;

    public StepRegist(DanceStep step, int timeMs)
    {
        danceStep = step;
        stepTimeMs = timeMs;
        life=stepTimeMs+BeatManager.Instance.marginMs/2;
    }

    public bool IsExpired(int nowMs)
    {
        return nowMs > life;
    }
}