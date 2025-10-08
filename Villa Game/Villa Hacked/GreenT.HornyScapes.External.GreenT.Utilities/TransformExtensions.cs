using UnityEngine;

namespace GreenT.HornyScapes.External.GreenT.Utilities;

public static class TransformExtensions
{
	public static void SetTransform(this Transform transform, Transform other)
	{
		transform.position = other.position;
		transform.rotation = other.rotation;
		transform.localScale = other.localScale;
	}
}
