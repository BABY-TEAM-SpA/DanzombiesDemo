using System.Collections.Generic;
using UnityEngine;

public class CanonPuzzle : RhythmPuzzle
{
    [SerializeField] private List<ZombieDanceBrain> zombies =new List<ZombieDanceBrain>();
    [SerializeField] List<SequenceStep> sequenceSteps = new List<SequenceStep>();
    int currentSequence = 0;
    List<DanceData> danceDatas = new List<DanceData>();
    
    [SerializeField] PlayerManager playerManager = new PlayerManager();
    
    
    public override void PreparePuzzle()
    {
        PlayerEnter(playerManager);
        danceDatas.Clear();
        currentSequence = 0;
        SetCounter(0);
        SetSequence(sequenceSteps[currentSequence]);
        if (zombies.Count > 0)
        {
            foreach (ZombieDanceBrain zombie in zombies)
            {
                DanceData data = new DanceData();
                data.listeners.AddListener(zombie);
                data.Sequence = sequenceSteps[currentSequence];
                danceDatas.Add(data);
            }
        }
        
    }
    
    public void OnDisable()
    {
        foreach (var danceData in danceDatas)
        {
            danceData.listeners.RemoveAllListeners();
        }
    }

    public override void ReactToPlayerStatus(DancerExpression.ExpressionType exp) { }

    public override void OnDanceSequenceCleared()
    {
        //currentSequence++;
        //SetSequence(sequenceSteps[currentSequence]);
        ActivatePuzzle(false);
    }

    public override void PreBeatAction(int counter)
    {
        PlayerHasDanced = false;
        int count = zombies.Count;
        foreach (var danceData in danceDatas)
        {
            danceData.SetDanceStep(counter+count);
            count--;
        }
        currentDanceData?.SetDanceStep(counter);
    }

    public override void BeatAction(int counter)
    {
        foreach (var danceData in danceDatas)
        {
            danceData.listeners.InvokeDance(danceData.DanceStep);
        }
        currentDanceData?.SetDanceStep(counter); 
    }

    public override void PostBeatAction(int counter)
    {
        currentDanceData?.SetFutureDanceStep(counter);
        CheckPlayerPost();
        currentDanceData.DanceStep = DanceStep.None;
        PlayerHasDanced = false;
    }
}
