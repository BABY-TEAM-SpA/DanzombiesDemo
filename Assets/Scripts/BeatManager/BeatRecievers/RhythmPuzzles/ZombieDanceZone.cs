using System;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDanceZone : RhythmPuzzle
{
    [Header("Shader Feedback Settings")]
    [SerializeField] private List<Color> gradientColors = new List<Color>(); // Lista de colores para el gradiente
    [SerializeField] private float pulseRiseSpeed = 8f;
    [SerializeField] private float preBeatPulse = 0.3f;
    [SerializeField] private float beatPulse = 1.2f;
    private Material zoneMaterial;
    private float currentPulse = 0f;
    private float targetPulse = 0f;
    
    [Header("Zombies Dance Settings")]
    [SerializeField] private List<ZombieDanceBrain> zombies = new List<ZombieDanceBrain>();
    public SequenceStep danceSequence;

    private void Start()
    {
        activeDanceSequence = danceSequence;
        if (feedBack != null && feedBack.material != null)
        {
            zoneMaterial = new Material(feedBack.material);
            feedBack.material = zoneMaterial;

            zoneMaterial.SetFloat("_ActiveState", 0f);  // Estado inicial en blanco

            float aspect = transform.localScale.x / transform.localScale.y;
            zoneMaterial.SetFloat("_Aspect", aspect);

            // Asegurarse de configurar los colores desde el inicio
            SetGradientColorsToMaterial();
        }

        foreach (ZombieDanceBrain zombie in zombies)
        {
            zombie.Connect(this);
        }
    }

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

    // Función para generar el color rainbow basado en el tiempo
    private Color GenerateRainbowColor(float time)
    {
        float hue = Mathf.Repeat(time * 0.1f, 1.0f); // Cambia el valor de 0.1f para velocidad
        float saturation = 1.0f;
        float value = 1.0f;

        // Convertir el valor HSV a RGB
        return Color.HSVToRGB(hue, saturation, value);
    }

    private void SetPulse(float value)
    {
        targetPulse = Mathf.Clamp(value, 0f, 2f);
    }

    private void OnDisable()
    {
        foreach (ZombieDanceBrain zombie in zombies)
        {
            zombie.Disconnect(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
        {
            if (zoneMaterial != null)
            {
                zoneMaterial.SetFloat("_ActiveState", 1f);  // Activar el rainbow cuando el jugador entra
            }

            PlayerEnter(player);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
        {
            if (zoneMaterial != null)
            {
                zoneMaterial.SetFloat("_ActiveState", 0f);  // Volver a blanco cuando el jugador sale
            }

            PlayerLeave(player);
        }
    }

    private void SetGradientColorsToMaterial()
    {
        if (gradientColors != null && gradientColors.Count > 0)
        {
            for (int i = 0; i < gradientColors.Count && i < 10; i++) // Usando un máximo de 10 colores
            {
                zoneMaterial.SetColor($"_ColorArray_{i}", gradientColors[i]);
            }

            zoneMaterial.SetInt("_NumColors", gradientColors.Count);  // Actualizando el número de colores
        }
    }

    public override void VisualFeedbackToPlayerDance(bool isCorrect)
    {
        // Mantengo tu override intacto
    }

    public override void ReactToPlayersDance(PlayerManager player, DanceStep step)
    {
        if (step == DanceStep.None)
            return;

        float flow = player.GetFlowDamage(player.saveDanceStep != step);

        if (flow < GameManager.Alza)
        {
            PlayerHasNoFlow(player);
        }
    }

    public override void PlayerHasNoFlow(PlayerManager player)
    {
        player.GetLifeDamage(true);
        PlayerLeave(player);
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

    public override void PreBeatAction(int counter)
    {
        base.PreBeatAction(counter);
        SetPulse(preBeatPulse);
    }

    public override void GeneralVisualFeedback(int counter)
    {
        SetPulse(beatPulse);
    }

    public override void PostBeatAction(int counter)
    {
        base.PostBeatAction(counter);
        SetPulse(0f);
    }
}