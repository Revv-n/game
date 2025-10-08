using System.Text;
using UnityEngine;

namespace GreenT.HornyScapes.Extensions;

public static class GameObjectExtensions
{
	public static bool IsDestroyed(this GameObject gameObject)
	{
		if (gameObject == null)
		{
			return (object)gameObject != null;
		}
		return false;
	}

	public static string GetFullPath(this Transform tr)
	{
		Transform[] componentsInParent = tr.GetComponentsInParent<Transform>();
		StringBuilder stringBuilder = new StringBuilder(componentsInParent[componentsInParent.Length - 1].name);
		for (int num = componentsInParent.Length - 2; num >= 0; num--)
		{
			stringBuilder.Append("/" + componentsInParent[num].name);
		}
		return stringBuilder.ToString();
	}
}
