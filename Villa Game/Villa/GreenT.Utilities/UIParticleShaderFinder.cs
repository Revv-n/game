using Coffee.UIExtensions;
using UnityEngine;

namespace GreenT.Utilities;

public class UIParticleShaderFinder : ShaderFinder
{
	[SerializeField]
	private UIParticle uiParticle;

	public void Awake()
	{
		foreach (Material material in uiParticle.materials)
		{
			ShaderFinder.UpdateMaterial(material);
		}
		foreach (ParticleSystem particle in uiParticle.particles)
		{
			ShaderFinder.UpdateMaterial(particle.GetComponent<ParticleSystemRenderer>().sharedMaterial);
		}
	}

	private void OnValidate()
	{
		if (!uiParticle)
		{
			uiParticle = GetComponent<UIParticle>();
		}
	}
}
