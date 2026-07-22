using UnityEngine;

public class Position3DGlobal : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private Position3D[] positions3D;
    #endregion

    #region [METHODS]
    public void FindChildrenPosition3D()
        => positions3D = GetComponentsInChildren<Position3D>(true);

    public void RefreshRenderers()
    {
        foreach (Position3D position3d in positions3D)
        {
            position3d.RefreshRenderers();
            position3d.SetLayerOnSprites();
        }
    }
    #endregion
}
