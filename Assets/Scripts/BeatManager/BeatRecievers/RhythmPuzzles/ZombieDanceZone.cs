using System;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDanceZone : RhythmPuzzle
{
    // Dehabilitamos el Feedback porque esto despues sera un script independiente enchufado a los puzzles.
    /* 
    [Header("Shader Feedback Settings")]
    [SerializeField] private List<Color> gradientColors = new List<Color>(); // Lista de colores para el gradiente
    [SerializeField] private float pulseRiseSpeed = 8f;
    [SerializeField] private float preBeatPulse = 0.3f;
    [SerializeField] private float beatPulse = 1.2f;
    private Material zoneMaterial;
    private float currentPulse = 0f;
    private float targetPulse = 0f;
    */
    
    [Header("Zombies Dance Settings")]
    [SerializeField] private List<ZombieDanceBrain> zombies = new List<ZombieDanceBrain>();
    public SequenceStep danceSequence;

    public override void PreparePuzzle()
    {
        SetSequence(danceSequence);
        //CurrentSequence.playbackMode = SequenceStep.PlaybackMode.Loop; /// ForceLoop on zombies
        
        // Dehabilitamos el Feedback porque esto despues sera un script independiente enchufado a los puzzles.
        /*if (feedBack != null && feedBack.material != null)
        {
            zoneMaterial = new Material(feedBack.material);
            feedBack.material = zoneMaterial;

            zoneMaterial.SetFloat("_ActiveState", 0f);  // Estado inicial en blanco

            float aspect = transform.localScale.x / transform.localScale.y;
            zoneMaterial.SetFloat("_Aspect", aspect);

            // Asegurarse de configurar los colores desde el inicio
            SetGradientColorsToMaterial();
        }*/

        foreach (ZombieDanceBrain zombie in zombies)
        {
            zombie.Connect(this);
        }
    }
    
    private void OnDisable()
    {
        foreach (ZombieDanceBrain zombie in zombies)zombie.Disconnect(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
        {
            //if (zoneMaterial != null) zoneMaterial.SetFloat("_ActiveState", 1f);
            PlayerEnter(player);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
        {
            // if (zoneMaterial != null) zoneMaterial.SetFloat("_ActiveState", 0f);  
            PlayerLeave(player);
        }
    }
    public override void PlayerEnter(PlayerManager player)
    {
        base.PlayerEnter(player);
        LevelUIController.Instance?.UpdateZombieFeedbackUI(true);
    }

    public override void PlayerLeave(PlayerManager player)
    {
        base.PlayerLeave(player);
        LevelUIController.Instance?.UpdateZombieFeedbackUI(false);
    }

    protected override void PuzzlePreBeat()
    {
        ////SetPulse(preBeatPulse);
    }

    protected override void PuzzleBeat(){ }

    protected override void PuzzlePostBeat()
    {
        ////SetPulse(0f);
    }

    public override void OnSequenceEnd()
    {
        Debug.Log("Puzzle End");
        ActivatePuzzle(false);
    }

    public override void OnUpdateSongAction(){ }
    
    // Dehabilitamos el Feedback porque esto despues sera un script independiente enchufado a los puzzles.
    /*
    private void Update()
    {
        
        if (zoneMaterial == null) return;

        // Generar color Rainbow dinámicamente (de acuerdo al tiempo)
        Color rainbowColor = GenerateRainbowColor(Time.time);

        // Si el jugador está en el área, aplicar el color rainbow
        if (zoneMaterial != null)
        {
            if (zoneMaterial.GetFloat("_ActiveState") == 1f)
            {
                zoneMaterial.SetColor("_RainbowColor", rainbowColor);
            }
            else
            {
                zoneMaterial.SetColor("_RainbowColor", Color.white);  // Si no está activo, fondo blanco
            }
        }

        // Actualización del pulso
        float speed = (currentPulse < targetPulse) ? pulseRiseSpeed : pulseRiseSpeed;
        currentPulse = Mathf.MoveTowards(currentPulse, targetPulse, speed * Time.deltaTime);

        zoneMaterial.SetFloat("_BeatPulse", currentPulse);
        
    }
    
    public override void GeneralVisualFeedback(int counter)
    {
        //SetPulse(beatPulse);
    }

    // Función para generar el color rainbow basado en el tiempo
    private Color GenerateRainbowColor(float time)
    {
        float hue = Mathf.Repeat(time * 0.1f, 1.0f); 
        float saturation = 1.0f;
        float value = 1.0f;

        // Convertir el valor HSV a RGB
        return Color.HSVToRGB(hue, saturation, value);
    }

    private void SetGradientColorsToMaterial()
    {
        if (gradientColors != null && gradientColors.Count > 0)
        {
            for (int i = 0; i < gradientColors.Count && i < 10; i++) zoneMaterial.SetColor($"_ColorArray_{i}", gradientColors[i]);
            zoneMaterial.SetInt("_NumColors", gradientColors.Count);
        }
    }
    
    private void SetPulse(float value)
    {
        targetPulse = Mathf.Clamp(value, 0f, 2f);
    }
    */
    
}