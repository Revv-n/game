using UnityEngine;

namespace GreenT.Utilities;

public static class CameraTools
{
	public static void FitCameraPosition(Camera camera, Bounds container)
	{
		Vector3 vector = BoundsExtension.BoundsShift(GetCamera2DWorldBounds(camera), container);
		camera.transform.position -= vector;
	}

	public static Vector4 GetCameraWorldBorders(Camera camera)
	{
		Vector3 vector = camera.ViewportToWorldPoint(Vector2.zero);
		Vector3 vector2 = camera.ViewportToWorldPoint(Vector2.one);
		return new Vector4(vector.x, vector.y, vector2.x, vector2.y);
	}

	public static Bounds GetCamera2DWorldBounds(Camera camera)
	{
		Vector3 vector = camera.ViewportToWorldPoint(Vector2.zero);
		Vector3 vector2 = camera.ViewportToWorldPoint(Vector2.one);
		Vector3 center = (vector2 + vector) / 2f;
		Vector3 size = vector2 - vector;
		return new Bounds(center, size);
	}
}
