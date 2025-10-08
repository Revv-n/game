using DG.Tweening;
using UnityEngine;

public static class EasyBezierTweener
{
	public static Tween DoBezier(Transform fly, Vector3 a, Vector3 b, float time, float extremumFactor = 0.2f, float extremumDistance = 0.5f, int preferedDirection = 0)
	{
		float num = Vector3.Distance(a, b);
		Vector3 vector = Vector3.Normalize(b - a);
		Vector3 vector2 = Vector2.Perpendicular(vector);
		if (preferedDirection > 0 && a.x > b.x)
		{
			vector2 *= -1f;
		}
		if (preferedDirection < 0 && a.x < b.x)
		{
			vector2 *= -1f;
		}
		Vector3 vector3 = a + vector * num * extremumDistance + vector2 * num * extremumFactor;
		Vector3 vector4 = a + vector2;
		Vector3 vector5 = b + vector2;
		Vector3 vector6 = vector3 - vector;
		Vector3 vector7 = vector3 + vector;
		fly.transform.position = a;
		return fly.DOPath(new Vector3[6] { vector3, vector4, vector6, b, vector7, vector5 }, time, PathType.CubicBezier);
	}
}
