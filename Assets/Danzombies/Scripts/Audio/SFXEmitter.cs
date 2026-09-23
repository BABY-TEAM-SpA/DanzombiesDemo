using FMOD;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;

/// <summary>
/// Componente para emitir SFXs usando FMOD. Permite reproducir, detener y controlar eventos del banco a través de sus parámetros y valores.
/// - eventRef: Referencia al evento de FMOD que se reproducirá.
/// - activeParam: COPIA del parámetro activo del evento de FMOD que se reproducirá. Se puede cambiar tanto el parámetro como su valor con métodos dedicados.
/// - sfxInstance: Instancia del evento de FMOD que se reproducirá.
/// 
/// Instrucciones de uso:
/// 1. Agregar SFXEmitter a un GameObject.
/// 2. Si cuenta con parámetros en FMOD, el campo paramters mostrará una lista; darle clic a Use en uno de los parámetros.
///    Esto copiará los datos del parámetro con su valor default a activeParam, lo que dejará a sfxInstance configurado para reproducirse.
/// 3. Para reproducir con el parámetro y valor del parámetro configurados, llamar a Play().
/// 4. Para cambiar el parámetro activo, llamar a SetParameter() o SetParameterByName() desde un UnityEvent.
/// 5. Para cambiar el valor del parámetro activo, llamar a UpdateParameterValue() desde un UnityEvent.
/// </summary>
public class SFXEmitter : MonoBehaviour
{
    #region [VARIABLES]
    private const float MIN_VOLUME = 0f;
    private const float MAX_VOLUME = 2f;

    public EventReference eventRef;
    public ParamRef activeParam; // <- Abstracción del parámetro del evento, NO es una referencia directa, ni siquiera una copia,
                                 //    porque no se clona a partir del evento; hay que verlo como un struct que ocupar en el evento real
    [Header("")]
    [SerializeField] private bool playOnStart;
    [SerializeField][Range(MIN_VOLUME, MAX_VOLUME)] private float volume = 1f;

    [Header("Spatializer")]
    [SerializeField] private bool overrideDistance;
    [Tooltip("Desde qué distancia se escucha con volumen máximo.")]
    [SerializeField][Range(0, 999)] private int minDistance = 0;
    [SerializeField][Range(1, 1000)] private int maxDistance = 1000;

    private EventInstance sfxInstance;
    #endregion

    #region [UNITY]
    private void Start()
    {
        if (eventRef.IsNull)
            return;

        sfxInstance = RuntimeManager.CreateInstance(eventRef);
        RuntimeManager.AttachInstanceToGameObject(sfxInstance, gameObject, GetComponent<Rigidbody2D>());

        ResolveParameterID();
        UpdateParameterValue(activeParam.Value);
        SetVolume(volume);
        if (overrideDistance)
            SetDistance(minDistance, maxDistance);

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
        if (sfxInstance.isValid())
            sfxInstance.start();
    }

    public void Stop(bool fadeOut)
    {
        if (sfxInstance.isValid())
            sfxInstance.stop(fadeOut ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
    }
    #endregion

    #region API - Parameters
    public void SetParameter(ParamRef param)
    {
        activeParam = param;

        if (sfxInstance.isValid())
            ResolveParameterID();
    }

    /// <summary>
    /// Método FF para el seteo de un nuevo parámetro de FMOD activo para el evento asignado a este SFXEmitter.
    /// Su propósito es permitir la existiencia de UpdateParameterValue, calleable desde los UnityEvent al no necesitar
    /// que se le diga explícitamente el parámetro a actualizar, solo su valor.
    /// </summary>
    public void SetParameterByName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        SetParameter(new ParamRef { Name = name, Value = 0f });
    }
    #endregion

    #region API - Parameter Value
    /// <summary>
    /// Método FF para la actualización del valor del parámetro de FMOD activo para el evento asignado a este SFXEmitter.
    /// La gracia es que transparenta el parámetro a actualizar, ya que se usa el activeParam, que también
    /// puede configurarse con SetParameter.
    /// </summary>
    public void UpdateParameterValue(float value)
    {
        if (activeParam.Name == string.Empty)
            return;

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

    #region API - Distance
    public void SetDistance(float min, float max)
    {
        if (!sfxInstance.isValid())
            return;

        RESULT resultMin = sfxInstance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, Mathf.Max(0f, min));
        RESULT resultMax = sfxInstance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, Mathf.Max(minDistance, max));

        if (resultMin != RESULT.OK || resultMax != RESULT.OK)
            Debug.LogWarning($"[SFXEmitter] No se pudo aplicar min/max distance. Min: {resultMin}, Max: {resultMax}.", this);
    }

    public void SetMinDistance(int min) => SetDistance(min, maxDistance);
    public void SetMaxDistance(int max) => SetDistance(minDistance, max);
    #endregion

    #region Helpers
    /// <summary>
    /// Método para resolver el ID del parámetro activo, a partir de su nombre, y actualizar su valor actual.
    /// Esta es la única forma de actualizar realmente el valor utilizado por el EventInstance. El resto de los métodos de la clase
    /// trabajan con el ParamRef en el inspector, así que es obligatorio pasar por ResolveParamterID para influir sobre el EventInstance.
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
