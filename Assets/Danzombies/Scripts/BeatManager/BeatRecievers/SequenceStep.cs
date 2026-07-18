using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum DanceStep
{
    None,
    L_North,
    R_North,
    L_South,
    R_South,
    L_West,
    R_West,
    L_East,
    R_East
}

[Serializable]
public class SequenceStep
{
    
    [Header("Pattern")]
    public BeatManager.BeatType patternBeatType = BeatManager.BeatType.FullBeat;
    public enum DamageMode
    {
        None,
        ModificaFlow,
        ModificaFlowYDaña
    }
    public DamageMode damageMode = DamageMode.None;
    [SerializeField]
    public List<DanceStep> pattern = new();

    public int startCounter { get; set; } = 0;
    
    public enum EndingMode
    {
        OneShot,
        StopOnXCorrectSteps,
        StopOnFullFlow,
        Loop,
        LoopShuffled,
    }
    [Header("Playback")]
    public EndingMode endingSeq = EndingMode.OneShot;
    [Min(1)] public int xCorrectSteps = 1;
    public enum StepsResetMode
    {
        None,
        DanceFail,
        SequenceReset
    }
    [SerializeField] private StepsResetMode ResetStepsCountOn  = StepsResetMode.None;
    private int playerCorrectDancesOnSequence = 0;
    
    public delegate void OnSeqEvent();
    public event OnSeqEvent OnDanceSequenceFinished;


    public void ApplyDance(bool isCorrect)
    {
        if (isCorrect)
        {
            playerCorrectDancesOnSequence += 1;
            switch (endingSeq)
            {
                case EndingMode.StopOnXCorrectSteps:
                    if (playerCorrectDancesOnSequence >= xCorrectSteps) StopSequence();
                    break;
                case EndingMode.StopOnFullFlow:
                    if(PlayerManager.Player.flow == 10) StopSequence();
                    break;
            }
        }
        if (ResetStepsCountOn == StepsResetMode.DanceFail) playerCorrectDancesOnSequence = 0;
    }
    
    public DanceStep GetDanceStep(int puzzleCounter)
    {
        DanceStep step = DanceStep.None;
        if (pattern.Count != 0 && puzzleCounter > 0) step = pattern[puzzleCounter % pattern.Count];
        return step;
    }
    
    public DanceStep GetFutureStep(int puzzleCounter)
    {
        int nextStepCounter = puzzleCounter+1;
        if(nextStepCounter >= pattern.Count) ResetSequence();
        return CalulateFutureStep(nextStepCounter);
    }

    private DanceStep CalulateFutureStep(int counter)
    {
        
        for (int i = 0; i < pattern.Count; i++)
        {
            int aux = i+counter;
            aux = aux % pattern.Count;
            if (pattern[aux] != DanceStep.None)
                return pattern[aux];
        }

        return DanceStep.None;
    }

    public void ShuffleSteps()
    {
        pattern = pattern.OrderBy(x => UnityEngine.Random.value).ToList();
    }

    public void ResetSequence()
    {
        Debug.Log("Reset Sequence");
        if(ResetStepsCountOn == StepsResetMode.SequenceReset) playerCorrectDancesOnSequence = 0;
        if(endingSeq == EndingMode.LoopShuffled) ShuffleSteps();
        if(endingSeq == EndingMode.OneShot) StopSequence();
    }

    public void StopSequence()
    {
        Debug.Log("Stop Sequence");
        OnDanceSequenceFinished?.Invoke();
        OnDanceSequenceFinished=null;
    }
}