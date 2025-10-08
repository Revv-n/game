using UnityEngine;

[ExecuteInEditMode]
public class _2DC_ShaderLerpDemo : MonoBehaviour
{
	public Material mat;

	public string variable;

	public AnimationCurve anm;

	public float Mul = 1f;

	public float Speed = 1f;

	private void Start()
	{
	}

	private void Update()
	{
		if (mat != null)
		{
			mat.SetFloat(variable, anm.Evaluate(Time.time * Speed) * Mul);
		}
	}
}
