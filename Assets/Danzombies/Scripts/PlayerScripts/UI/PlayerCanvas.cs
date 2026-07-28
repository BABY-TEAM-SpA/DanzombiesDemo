using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Canvas de UI sobre Greg.
/// </summary>
public class PlayerCanvas : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private PlayerManager player;

    [Header("Elements")]
    public TwistedHP TwistedHP;
    #endregion

    #region [METHODS]
    public void SetHP() => TwistedHP?.SetHP(player.HP);
    #endregion
}
