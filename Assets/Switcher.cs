using System.Collections.Generic;
using UnityEngine;

public enum ActivatorBehavior
{
    Single,
    Additive,
}

public class Switcher : MonoBehaviour
{
    public BeatManager.BeatType updateMethod;
    public ActivatorBehavior activatorBeatBehavior = ActivatorBehavior.Single;
    public List<GameObject> objects = new List<GameObject>();
    private int index = -1;

    public void ResetCounter()
    {
        index = -1;
    }

    public void ActivateByRhythm(BeatManager.BeatType beatType, DanceStep danceStep)
    {
        if(beatType == BeatManager.BeatType.FullBeat) Activate();
        //ventana abierta para nuevos metodos
    }
    
    public void Activate()
    {
        if (index<0) index=0;
        if (activatorBeatBehavior == ActivatorBehavior.Single && index>1) objects[index-1].gameObject.SetActive(false);
        if (index < objects.Count) objects[index].gameObject.SetActive(true);
        if((index >= 0 && index < objects.Count)) index++;
        else index = 0;
    }

    public void DeactivateAll()
    {
        foreach (var obj in objects)
        {
            obj.SetActive(false);
        }
    }
}
