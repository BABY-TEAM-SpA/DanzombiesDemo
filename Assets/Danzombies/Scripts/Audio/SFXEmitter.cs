using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class SFXEmitter : MonoBehaviour
{
    #region [VARIABLES]
    private const float MIN_VOLUME = 0f;
    private const float MAX_VOLUME = 2f;

    public EventReference eventRef;
    public ParamRef activeParam; // <- Abstracción del parámetro del evento, NO es una referencia directa, ni siquiera una copia,
                                 //    porque no se clona a partir del evento; hay que verlo como un struct que ocupar en el evento real
    [SerializeField] private bool playOnStart;
    [SerializeField][Range(MIN_VOLUME, MAX_VOLUME)] private float volume = 1f;

    private EventInstance sfxInstance;
    #endregion

    #region [UNITY]
    private void Start()
    {
        if (eventRef.IsNull)
            return;

        sfxInstance = RuntimeManager.CreateInstance(eventRef);

        ResolveParameterID();
        UpdateParameterValue(activeParam.Value);
        SetVolume(volume);

        if (playOnStart)
            Play();
    }

    private void OnDestroy()
    {
        if (sfxInstance.isValid())
            sfxInstance.release();
    }
    #endregion

    #region [METHODS]
    #region API - Studio
    public void Play()
    {
        if (!sfxInstance.isValid())
            return;

        sfxInstance.start();
        RuntimeManager.AttachInstanceToGameObject(sfxInstance, gameObject, GetComponent<Rigidbody2D>());
    }

    public void Stop()
    {
        if (!sfxInstance.isValid())
            return;

        sfxInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }
    #endregion

    #region API - Parameters
    /// <summary>
    /// Método FF para el seteo de un nuevo parámetro de FMOD activo para el evento asignado a este SFXEmitter.
    /// Su propósito es permitir la existiencia de UpdateParameterValue, calleable desde los UnityEvent al no necesitar
    /// que se le diga explícitamente el parámetro a actualizar, solo su valor.
    /// </summary>
    public void SetParameter(ParamRef param)
    {
        activeParam = param;

        if (sfxInstance.isValid())
            ResolveParameterID();
    }

    public void SetParameterByName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        SetParameter(new ParamRef { Name = name, Value = 0f });
    }

    /// <summary>
    /// Método FF para la actualización del valor del parámetro de FMOD activo para el evento asignado a este SFXEmitter.
    /// La gracia es que transparenta el parámetro a actualizar, ya que se usa el activeParam, que también
    /// puede configurarse con SetParameter.
    /// </summary>
    public void UpdateParameterValue(float value)
    {
        if (activeParam == null)
        {
            Debug.LogWarning($"[SFXEmitter] El parámetro activo es null, cancelando operación.", this);
            return;
        }

        RESULT result = sfxInstance.setParameterByID(activeParam.ID, value);
        if (result != RESULT.OK)
        {
            Debug.LogWarning($"[SFXEmitter] Resultado: {result}.", this);
            return;
        }

        activeParam.Value = value;
    }
    #endregion

    #region API - Volume
    public void SetVolume(float value)
    {
        volume = Mathf.Clamp(value, MIN_VOLUME, MAX_VOLUME);
        
        if (sfxInstance.isValid())
            sfxInstance.setVolume(volume);
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Método para resolver el ID del parámetro activo, a partir de su nombre, y actualizar su valor actual.
    /// Esta es la única forma de actualizar realmente el valor utilizado por el EventInstance. El resto de los métodos
    /// solo actualizan el valor del ParamRef, el cual se usa en este método para actuar sobre EventInstance.
    /// </summary>
    private void ResolveParameterID()
    {
        if (activeParam == null || string.IsNullOrEmpty(activeParam.Name))
            return;

        RESULT result = sfxInstance.getDescription(out EventDescription description);
        if (result != RESULT.OK)
        {
            Debug.LogWarning($"[SFXEmitter] Resultado: {result}.", this);
            return;
        }

        result = description.getParameterDescriptionByName(activeParam.Name, out PARAMETER_DESCRIPTION paramDescription);
        if (result != RESULT.OK)
        {
            Debug.LogWarning($"[SFXEmitter] Resultado: {result}.", this);
            return;
        }

        activeParam.ID = paramDescription.id;
    }
    #endregion
    #endregion
}
