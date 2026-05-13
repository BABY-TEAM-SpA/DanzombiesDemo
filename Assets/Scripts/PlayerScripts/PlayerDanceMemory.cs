using UnityEngine;
using System.Collections.Generic;

public class PlayerDanceMemory : MonoBehaviour
{
    int stepMemorySize;

    List<DanceStep> stepMemory = new List<DanceStep>();

    void Start()
    {
        InitializeMemory();
    }

    public void RememberStep(PlayerManager player, DanceStep step)
    {
        if (stepMemorySize == 0) return;
        stepMemory.Add(step);
        if (stepMemory.Count > stepMemorySize)
        {
            stepMemory.RemoveAt(0);
        }
    }

    public void InitializeMemory()
    {
        stepMemory = new List<DanceStep>();
        for(int i = 0; i < stepMemorySize; i++)
        {
            stepMemory.Add(DanceStep.None);
        }
    }

    public void SetMemorySize(int newSize)
    {
        InitializeMemory();
        if (newSize < 0) newSize = 0;
        stepMemorySize = newSize;
    }

    public bool MatchesMemory(List<DanceStep> sequence)
    {
        if (sequence.Count > stepMemorySize)
        {
            Debug.LogWarning("[PlayerDanceMemory]: Recibida una sequencia de pasos mas grande que la memoria disponible");
            return false;
        }
        int i = sequence.Count;
        int j = stepMemory.Count;
        while(i < 0)
        {
            if(sequence[i-1] != stepMemory[j-1])
                return false;
            i--;
            j--;
        }
        return true;
    }
}