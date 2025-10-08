using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.Utility;

public static class TweenHelper
{
	public static Sequence ChangeAlpha(IEnumerable<MaskableGraphic> content, float animationDuration, float to)
	{
		Sequence sequence = DOTween.Sequence();
		foreach (MaskableGraphic item in content)
		{
			sequence.Insert(0f, DOTweenModuleUI.DOFade(item, to, animationDuration));
		}
		return sequence;
	}

	public static Color Alpha(Material material, float alpha)
	{
		Color color = material.color;
		color.a = alpha;
		return color;
	}

	public static Color Alpha(Image image, float alpha)
	{
		Color color = image.color;
		color.a = alpha;
		return color;
	}
}
