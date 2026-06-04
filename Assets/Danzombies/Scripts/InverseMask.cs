using UnityEngine;
using UnityEngine.UI;

public class InverseMask : Image
{
    public override Material materialForRendering
    {
        get
        {
            var mat = new Material(base.materialForRendering);
            mat.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.NotEqual);
            return mat;
        }
    }
}
