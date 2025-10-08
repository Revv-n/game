using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleShaderFinder : MonoBehaviour
{
	[SerializeField]
	private ParticleSystemRenderer particleSystem;

	private void Awake()
	{
		Shader shader = Shader.Find(((Renderer)(object)particleSystem).sharedMaterial.shader.name);
		((Renderer)(object)particleSystem).sharedMaterial.shader = shader;
	}

	private void OnValidate()
	{
		if (!(Object)(object)particleSystem)
		{
			particleSystem = GetComponent<ParticleSystemRenderer>();
		}
	}
}
