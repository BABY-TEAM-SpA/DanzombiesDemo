using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class DanceLevelController : MonoBehaviour
{
    

    [SerializeField] private List<Image> Barras =new List<Image>();
    
    [SerializeField] private Color SeguridadNormal = Color.white;
    [SerializeField] private Color SeguridadInsecure = Color.white;
    [SerializeField] private Color SeguridadFlow = Color.white;

    public UnityEvent OnPlayerDies;
    public static DanceLevelController Instance { get; private set; }
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
        }
        
    }

    

    private void HandleBarras(float value)
    {
        foreach (Image barra in Barras)
        {
            barra.fillAmount = value;
            if (value < 0.2f)
            {
                barra.color = SeguridadInsecure;
            }
            else if (value <= 0.5f)
            {
                barra.color = SeguridadNormal;
            }
            else
            {
                barra.color = SeguridadFlow;
            }
        }
    }
}
