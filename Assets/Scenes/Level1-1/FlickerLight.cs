using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;

public class FlickerLight : MonoBehaviour
{
    [SerializeField,Min(0f)] private float minIntensity = 0.5f;
    private float maxIntensity = 1.2f;
    [SerializeField, Min(0f)] private float timeBetweenIntensity = 0.1f;

    private Light2D lightToFlicker;
    private float currentTimer;

    private void OnValidate()
    {
        maxIntensity = Mathf.Max(maxIntensity, minIntensity);
    }
    private void Awake()
    {
        if (lightToFlicker == null)
        {
            lightToFlicker = GetComponent<Light2D>();
            maxIntensity = lightToFlicker.intensity;   
        }
        
    }

    private void Update()
    {
        currentTimer+= Time.deltaTime;
        if (!(currentTimer >= timeBetweenIntensity)) return;
        lightToFlicker.intensity = Random.Range(minIntensity, maxIntensity);
        currentTimer = 0;

    }
}
