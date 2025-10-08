using UnityEngine;

namespace GreenT.Utilities;

public abstract class ShaderFinder : MonoBehaviour
{
	public static void UpdateMaterial(Material material)
	{
		Shader shader = Shader.Find(material.shader.name);
		material.shader = null;
		material.shader = shader;
	}
}
