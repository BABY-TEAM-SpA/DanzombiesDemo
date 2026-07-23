using System.Collections.Generic;
using UnityEngine;

public class TutorialPuzzle : ZombieDanceZone
{
    #region [VARIABLES]
    [SerializeField] private ZombieDanceBrain Steph;
    [SerializeField] private TutorialDanceBrain HUD;
    public int puzzleGoal;
    [HideInInspector] public int currentTutorialSequence = 0;
    
    [Header("Tutorial Dance Settings")]
    public List<SequenceStep> TutorialSequences = new List<SequenceStep>();
    
    #endregion

    #region [UNITY]
    private void OnDisable()
    {
        currentDanceData.listeners.RemoveAllListeners();
    }
    #endregion

    #region [METHODS]
    public override void PreparePuzzle()
    {
        currentDanceData.listeners.AddListener(Steph);
        currentDanceData.listeners.AddListener(HUD);
    }

    public override void ActivatePuzzle(bool activate)
    {
        HUD.SetActiveCanvas(activate);
        base.ActivatePuzzle(activate);
        if (currentTutorialSequence < TutorialSequences.Count) SetSequence(TutorialSequences[currentTutorialSequence]);
    }


    public override void ReactToPlayerStatus(DancerExpression.ExpressionType exp)
    {
        //Nothing to do
    }

    public override void OnDanceSequenceCleared()
    {
        Debug.Log("OnDanceSequenceCleared");
        currentTutorialSequence += 1;
        ActivatePuzzle(false);

    }
    #endregion
}
