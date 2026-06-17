using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    //SINGLETON
    public static ScoreManager Instance { get; private set; }
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this.gameObject); 
        } 
        else 
        { 
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        
    }
    //SINGLETON END

    public int currentScore {get; private set;}
    public int currentMult {get; private set;}

    [SerializeField] UnityEvent OnMultIncrease;
    [SerializeField] UnityEvent OnMultDecrease;
    [SerializeField] UnityEvent OnScoreIncrease;
    [SerializeField] UnityEvent OnScoreDecrease;

    public void SetScore(int newScore)
    {
        if (newScore == currentScore) return;
        bool isHigher = (newScore > currentScore);
        currentScore = newScore;
        if (isHigher)
        {
            OnScoreIncrease?.Invoke();
        }
        else
        {
            OnScoreDecrease?.Invoke();
        }
    }

    public void AddScore(int toAdd, bool useMult = true) //can be negative
    {
        toAdd = toAdd * (useMult ? currentMult : 0);
        if (toAdd == 0) return;

        currentScore += toAdd;
        if (toAdd > 0)
        {
            OnScoreIncrease?.Invoke();
        }
        else
        {
            OnScoreDecrease?.Invoke();
        }
    }

    public void SetMult(int newMult)
    {
        if (newMult == currentMult) return;
        bool isHigher = (newMult > currentMult);
        currentMult = newMult;
        if (isHigher)
        {
            OnMultIncrease?.Invoke();
        }
        else
        {
            OnMultDecrease?.Invoke();
        }
    }

    public void AddMult(int toAdd) //can be negative
    {
        if (toAdd == 0) return;
        currentMult += toAdd;
        if (toAdd > 0)
        {
            OnMultIncrease?.Invoke();
        }
        else
        {
            OnMultDecrease?.Invoke();
        }
    }

}
