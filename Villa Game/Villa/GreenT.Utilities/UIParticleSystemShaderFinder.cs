using UnityEngine;

namespace GreenT.Utilities;

public class UIParticleSystemShaderFinder : ShaderFinder
{
	[SerializeField]
	private UIParticleSystem uiParticleSystem;

	public void Awake()
	{
		ReplaceShaderByName();
	}

	public void ReplaceShaderByName()
	{
		ShaderFinder.UpdateMaterial(uiParticleSystem.material);
	}

	private void OnValidate()
	{
		if (!uiParticleSystem)
		{
			uiParticleSystem = GetComponent<UIParticleSystem>();
		}
	}
}
