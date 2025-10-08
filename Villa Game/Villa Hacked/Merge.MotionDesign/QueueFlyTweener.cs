using System;
using DG.Tweening;
using UnityEngine;

namespace Merge.MotionDesign;

public static class QueueFlyTweener
{
	public static Tween DoQueueFly(Func<GameObject> itemCtorFunc, Func<GameObject, Tween> flyTweenFunc, int count = 5, float totalTime = 1f, AnimationCurve spawningCurve = null)
	{
		if (spawningCurve == null)
		{
			spawningCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		}
		Sequence sequence = DOTween.Sequence();
		for (int i = 0; i < count; i++)
		{
			GameObject item = itemCtorFunc();
			item.SetActive(value: false);
			Sequence sequence2 = DOTween.Sequence();
			sequence2.AppendCallback(delegate
			{
				item.SetActive(value: true);
			});
			sequence2.Append(flyTweenFunc(item));
			sequence2.AppendCallback(delegate
			{
				UnityEngine.Object.Destroy(item);
			});
			float time = (float)i / (float)count;
			float atPosition = spawningCurve.Evaluate(time);
			sequence.Insert(atPosition, sequence2);
		}
		return sequence;
	}
}
