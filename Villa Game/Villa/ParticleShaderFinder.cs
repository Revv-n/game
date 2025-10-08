using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleShaderFinder : MonoBehaviour
{
	[SerializeField]
	private ParticleSystemRenderer particleSystem;

	private void Awake()
	{
		Shader shader = Shader.Find(particleSystem.sharedMaterial.shader.name);
		particleSystem.sharedMaterial.shader = shader;
	}

	private void OnValidate()
	{
		if (!particleSystem)
		{
			particleSystem = GetComponent<ParticleSystemRenderer>();
		}
	}
}
