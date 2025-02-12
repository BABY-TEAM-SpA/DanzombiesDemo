using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceZone : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.TryGetComponent<DanceBrain>(out DanceBrain brain))
        {
            Debug.Log("Brain suscribed");
            //brain
        }
    }
}
