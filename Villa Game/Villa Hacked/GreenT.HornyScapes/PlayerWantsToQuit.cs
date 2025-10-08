using System;
using UnityEngine;

namespace GreenT.HornyScapes;

public static class PlayerWantsToQuit
{
	public static Action<bool> OnWantsToQuit;

	public static bool AllowQuitting;

	private static bool WantsToQuit()
	{
		Debug.Log("Player prevented from quitting. AllowQuitting = false");
		OnWantsToQuit?.Invoke(AllowQuitting);
		return AllowQuitting;
	}

	[RuntimeInitializeOnLoadMethod]
	private static void RunOnStart()
	{
		Application.wantsToQuit += WantsToQuit;
	}
}
