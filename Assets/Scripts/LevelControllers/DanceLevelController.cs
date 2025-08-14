using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum SeguridadState
{
    Normal,
    Insecure,
    Flow
}
public class DanceLevelController : MonoBehaviour
{
    [SerializeField] [Range(0f,1f)] private float NivelDeSeguridad = 0.5f;
    [SerializeField] [Range(0f,0.25f)] private float Alza = 0.1f;

    [SerializeField] private List<Image> Barras =new List<Image>();
    public SeguridadState currenState { get; private set; } = SeguridadState.Normal;
    [SerializeField] private Color SeguridadNormal = Color.white;
    [SerializeField] private Color SeguridadInsecure = Color.white;
    [SerializeField] private Color SeguridadFlow = Color.white;

    public UnityEvent OnLifeEnd;
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
    private void Update()
    {
        foreach (Image barra in Barras)
        {
            HandleBarras(barra);
        }
    }

    public void ModificarVida(bool modificar)
    {
        NivelDeSeguridad=Mathf.Clamp((modificar)?NivelDeSeguridad+Alza:NivelDeSeguridad-Alza,0f,1f);
    }

    private void HandleBarras(Image barra)
    {
        barra.fillAmount = NivelDeSeguridad;
        if (NivelDeSeguridad < 0.2f)
        {
            //Inseguro
            currenState = SeguridadState.Insecure;
            barra.color = SeguridadInsecure;
        }
        else if (NivelDeSeguridad <= 0.8f)
        {
            //Normal
            currenState = SeguridadState.Normal;
            barra.color = SeguridadNormal;
        }
        else
        {
            //FLow
            currenState = SeguridadState.Flow;
            barra.color = SeguridadFlow;
        }
        if (NivelDeSeguridad <= 0.05f)
        {
            OnLifeEnd?.Invoke();
        }
    }
}
