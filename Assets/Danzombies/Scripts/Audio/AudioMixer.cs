using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioMixer : Service<AudioMixer>
{
    #region [VARIABLES]
    public Bus MasterBus { get; private set; }
    public Bus MusicBus { get; private set; }
    public Bus SFXBus { get; private set; }
    public Bus UIBus { get; private set; }
    public Bus OtherBus { get; private set; }
    #endregion

    #region [UNITY]
    protected override void Awake()
    {
        base.Awake();

        MasterBus = RuntimeManager.GetBus("bus:/");
        MusicBus = RuntimeManager.GetBus("bus:/Music");
        SFXBus = RuntimeManager.GetBus("bus:/SFX");
        UIBus = RuntimeManager.GetBus("bus:/UI");
        OtherBus = RuntimeManager.GetBus("bus:/Other");
    }
    #endregion

    #region [METHODS]
    #region API
    public void SetMasterVolume(float volume) => MasterBus.setVolume(volume);
    public void SetMusicVolume(float volume) => MusicBus.setVolume(volume);
    public void SetSFXVolume(float volume) => SFXBus.setVolume(volume);
    public void SetUIVolume(float volume) => UIBus.setVolume(volume);
    public void SetOtherVolume(float volume) => OtherBus.setVolume(volume);
    #endregion
    #endregion
}
