using UnityEngine;

namespace UniversalAmplitude;

public static class DemoExtensions
{
	public static void HideImmediate(this CanvasGroup cg)
	{
		cg.alpha = 0f;
		cg.blocksRaycasts = false;
		cg.interactable = false;
	}

	public static void ShowImmediate(this CanvasGroup cg)
	{
		cg.alpha = 1f;
		cg.blocksRaycasts = true;
		cg.interactable = true;
	}

	public static void UpdateOpacity(this CanvasGroup cg, bool shouldShow, float fadeSpeed)
	{
		if (shouldShow && (cg.alpha < 1f || !cg.interactable))
		{
			cg.alpha += fadeSpeed * Time.deltaTime;
			cg.interactable = true;
			cg.blocksRaycasts = true;
		}
		else if (!shouldShow && (cg.alpha > 0f || cg.interactable))
		{
			cg.alpha -= fadeSpeed * Time.deltaTime;
			cg.interactable = false;
			cg.blocksRaycasts = false;
		}
	}
}
