using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;

public class FlickerLight : BeatReciever
{
    [SerializeField,Min(0f)] private float minIntensity = 0.5f;
    private float maxIntensity = 1.2f;
    public bool useBeatTime;
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
        if (useBeatTime) return;
        currentTimer+= Time.deltaTime;
        if (!(currentTimer >= timeBetweenIntensity)) return;
        currentTimer = 0;
        Flick();
    }

    private void Flick()
    {
        lightToFlicker.intensity = Random.Range(minIntensity, maxIntensity);
    }

    public override void PreBeatAction(int beat, BeatManager.BeatType type)
    {
        //throw new System.NotImplementedException();
    }

    public override void BeatAction(int beat, BeatManager.BeatType type)
    {
        if(useBeatTime && type == BeatManager.BeatType.FullBeat) Flick();
    }

    public override void PostBeatAction(int beat, BeatManager.BeatType type)
    {
        //throw new System.NotImplementedException();
    }
}
