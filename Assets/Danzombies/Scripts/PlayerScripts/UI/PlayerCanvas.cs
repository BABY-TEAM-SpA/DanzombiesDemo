using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Canvas de UI sobre Greg.
/// </summary>
public class PlayerCanvas : MonoBehaviour
{
    #region [VARIABLES]
    public TwistedHP TwistedHP;
    #endregion

    #region [UNITY]
    #endregion

    #region [METHODS]
    public void SetHP(int hp) => TwistedHP?.SetHP(hp);

    public void TurnOn(Type type)
    {
        if (type == typeof(TwistedHP))
            TwistedHP?.TurnOn();
    }

    public void TurnOff(Type type)
    {
        if (type == typeof(TwistedHP))
            TwistedHP?.TurnOff();
    }
    #endregion
}
