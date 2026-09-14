using System.Collections.Generic;
using UnityEngine;

public enum ActivatorBehavior
{
    Single,
    Additive,
}

public class Switcher : MonoBehaviour
{
    public ActivatorBehavior activatorBeatBehavior = ActivatorBehavior.Single;
    public List<GameObject> objects = new List<GameObject>();
    private int index = -1;

    public void ResetCounter()
    {
        index = -1;
    }
    
    public void Activate()
    {
        if (index<0) index=0;
        if (index < objects.Count)
        {
            objects[index].gameObject.SetActive(true);
        }
    }
    

    public void SetNext()
    {
        if((index >= 0 && index < objects.Count))
        {
            if (activatorBeatBehavior == ActivatorBehavior.Single) objects[index].gameObject.SetActive(false);
            index++;
        }
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
