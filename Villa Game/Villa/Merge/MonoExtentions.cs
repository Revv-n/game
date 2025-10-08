using System;
using System.Collections;
using UnityEngine;

namespace Merge;

public static class MonoExtentions
{
	public static void DelayedCall(this MonoBehaviour mb, Action callback, float seconds = 0f)
	{
		mb.StartCoroutine(mb.CRT_DelayedCall(callback, seconds));
	}

	private static IEnumerator CRT_DelayedCall(this MonoBehaviour mb, Action callback, float seconds = 0f)
	{
		yield return (seconds == 0f) ? null : new WaitForSeconds(seconds);
		callback?.Invoke();
	}
}
