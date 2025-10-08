using System;
using UnityEngine;

namespace GreenT.HornyScapes;

public class ResolutionSetter : MonoBehaviour
{
	private const FullScreenMode FullScreenMode = FullScreenMode.FullScreenWindow;

	private static void Set()
	{
		int width = Screen.width;
		int height = Screen.height;
		int num = 16;
		int num2 = 9;
		double num3 = Math.Min((double)width / (double)num, (double)height / (double)num2);
		int width2 = (int)((double)num * num3);
		int height2 = (int)((double)num2 * num3);
		Screen.SetResolution(width2, height2, FullScreenMode.FullScreenWindow);
	}

	private void Awake()
	{
		if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			Set();
		}
	}
}
