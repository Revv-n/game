using UnityEngine;

namespace GreenT.HornyScapes.Tools;

public class VisibilityChecker
{
	public bool IsVisible(Camera camera, float cameraExtentsCoef, Vector3 objectPosition, Vector3 extents)
	{
		float num = camera.orthographicSize * 2f * cameraExtentsCoef;
		float num2 = num * camera.aspect * cameraExtentsCoef;
		Vector3 position = camera.transform.position;
		float num3 = position.x - num2 / 2f;
		float num4 = position.x + num2 / 2f;
		float num5 = position.y - num / 2f;
		float num6 = position.y + num / 2f;
		Vector3 vector = objectPosition - extents;
		Vector3 vector2 = objectPosition + extents;
		if (!(vector2.x < num3) && !(vector.x > num4) && !(vector2.y < num5))
		{
			return !(vector.y > num6);
		}
		return false;
	}
}
