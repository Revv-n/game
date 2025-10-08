using UnityEngine;

namespace GreenT.HornyScapes;

public class CopyUtil
{
	public static void Copy(string text)
	{
		GUIUtility.systemCopyBuffer = text;
	}
}
