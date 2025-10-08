using GreenT.Utilities;
using UnityEngine;

public class RendererShaderFinder : ShaderFinder
{
	[SerializeField]
	private Renderer renderer;

	public void Awake()
	{
		Material[] sharedMaterials = renderer.sharedMaterials;
		for (int i = 0; i < sharedMaterials.Length; i++)
		{
			ShaderFinder.UpdateMaterial(sharedMaterials[i]);
		}
	}

	private void OnValidate()
	{
		if (renderer == null)
		{
			TryGetComponent<Renderer>(out renderer);
		}
	}
}
