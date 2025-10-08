using UnityEngine;
using UnityEngine.UI;

namespace GreenT.Utilities;

public class GraphicShaderFinder : ShaderFinder
{
	[SerializeField]
	private Graphic graphic;

	public void Awake()
	{
		ShaderFinder.UpdateMaterial(graphic.material);
	}

	private void OnValidate()
	{
		if (!graphic)
		{
			graphic = GetComponent<Graphic>();
		}
	}
}
