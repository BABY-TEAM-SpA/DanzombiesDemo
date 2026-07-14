using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;

public class FlickerLight : MonoBehaviour
{
    
    private int innerCounter;
    [SerializeField,Min(0f)] private float minIntensity = 0.5f;
    private float maxIntensity = 1.2f;
    [SerializeField, Min(0f)] private float timeBetweenIntensity = 0.1f;
    public bool useBeatTime;
    public BeatManager.BeatType beatType;
    [Min(1)] public int beatMarginUpdate =1;

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
        
        if (useBeatTime)
        {
            int counter = BeatManager.Instance.GetCounter(beatType);
            if ( counter <= innerCounter+beatMarginUpdate-1) return;
            innerCounter=counter;
        }
        else
        {
            currentTimer+= Time.deltaTime;
            if (!(currentTimer >= timeBetweenIntensity)) return;
            currentTimer = 0;
        }
        lightToFlicker.intensity = Random.Range(minIntensity, maxIntensity);
        

    }
}
