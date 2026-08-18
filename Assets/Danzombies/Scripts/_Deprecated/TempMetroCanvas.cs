using UnityEngine;

/// <summary>
/// SCRIPT TEMPORAL, SERÁ ELIMINADO CON EL REWORK DE LOS RESETTABLES.
/// </summary>
public class TempMetroCanvas : MonoBehaviour
{
    [SerializeField] private ZombieChasingHordeBehaviour horde;

    private void Start() => horde.SetChase(false);
}
