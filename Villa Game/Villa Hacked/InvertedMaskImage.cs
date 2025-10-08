using UnityEngine;
using UnityEngine.UI;

public class InvertedMaskImage : Image
{
	public override Material materialForRendering
	{
		get
		{
			Material obj = Object.Instantiate(base.materialForRendering);
			obj.SetInt("_StencilComp", 6);
			return obj;
		}
	}
}
