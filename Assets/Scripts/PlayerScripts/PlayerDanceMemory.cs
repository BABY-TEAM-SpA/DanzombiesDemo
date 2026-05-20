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
        if (newSize < 0) newSize = 0;
        stepMemorySize = newSize;
        InitializeMemory();
    }

    public bool MatchesMemory(List<DanceStep> sequence) //checkea si la secuencia recibida es igual a la que se mantiene en memoria
    //para la memoria solo importan los ultimos pasos; osea, si en memoria hay 10 pasos, y la secuencia recibida solo tiene 3
    //se checkean los ultimos 3 pasos de la memoria, y el resto pueden ser cualquiera
    {
        if (sequence.Count > stepMemorySize)
        {
            Debug.LogWarning("[PlayerDanceMemory]: Recibida una secuencia de pasos mas grande que la memoria disponible");
            return false;
        }
        int i = sequence.Count;
        int j = stepMemory.Count;
        while(i > 0)
        {
            if(sequence[i-1] != stepMemory[j-1])
                return false;
            i--;
            j--;
        }
        return true;
    }
}