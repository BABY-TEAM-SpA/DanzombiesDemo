using UnityEngine;

public class Door : Interactive
{
    #region [VARIABLES]
    #endregion

    #region [METHODS]
    public override void Interact(PlayerManager player)
    {
        Debug.Log("Opening door");
    }
    #endregion
}
