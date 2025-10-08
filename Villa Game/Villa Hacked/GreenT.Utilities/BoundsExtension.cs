using UnityEngine;

namespace GreenT.Utilities;

public static class BoundsExtension
{
	public static Bounds Intersection(Bounds bounds1, Bounds bounds2)
	{
		Vector3 vector = Vector3.Max(bounds1.min, bounds2.min);
		Vector3 vector2 = Vector3.Min(bounds1.max, bounds2.max);
		return new Bounds((vector + vector2) / 2f, vector2 - vector);
	}

	public static Vector2 BoundsShift(Bounds source, Bounds container)
	{
		float x = 0f;
		float y = 0f;
		if (source.max.x - container.max.x > 0f)
		{
			x = Mathf.Clamp(source.max.x - container.max.x, 0f, source.min.x - container.min.x);
		}
		else if (source.min.x - container.min.x < 0f)
		{
			x = 0f - Mathf.Clamp(container.min.x - source.min.x, 0f, container.max.x - source.max.x);
		}
		if (source.max.y - container.max.y > 0f)
		{
			y = Mathf.Clamp(source.max.y - container.max.y, 0f, source.min.y - container.min.y);
		}
		else if (source.min.y - container.min.y < 0f)
		{
			y = 0f - Mathf.Clamp(container.min.y - source.min.y, 0f, container.max.y - source.max.y);
		}
		return new Vector2(x, y);
	}
}
