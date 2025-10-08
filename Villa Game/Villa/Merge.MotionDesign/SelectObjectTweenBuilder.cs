using System;
using DG.Tweening;
using UnityEngine;

namespace Merge.MotionDesign;

public class SelectObjectTweenBuilder : MonoBehaviour
{
	[Serializable]
	public class LockedTweenInfo
	{
		public float moveTime = 0.12f;

		public float moveDistance = 0.2f;

		public float decayFactor = 0.8f;

		public float movesCount = 4f;

		public Ease ease = Ease.InOutSine;
	}

	public float time;

	public float max_size_time;

	public float min_size_time;

	public float min_scale;

	public float max_scale;

	[SerializeField]
	private LockedTweenInfo lockedTweenInfo;

	public Tween BuildNormalTween(Transform tr)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Append(tr.DOScale(min_scale, min_size_time));
		sequence.Append(tr.DOScale(max_scale, max_size_time));
		sequence.Append(tr.DOScale(1f, time));
		return sequence;
	}

	public Tween BuildLockTween(Transform tr)
	{
		Sequence sequence = DOTween.Sequence();
		float num = lockedTweenInfo.moveDistance * (float)((UnityEngine.Random.value > 0.5f) ? 1 : (-1));
		for (int i = 0; (float)i < lockedTweenInfo.movesCount; i++)
		{
			sequence.Append(tr.DOLocalMoveX(num, lockedTweenInfo.moveTime).SetEase(lockedTweenInfo.ease));
			num = num * lockedTweenInfo.decayFactor * -1f;
		}
		sequence.Append(tr.DOLocalMoveX(0f, lockedTweenInfo.moveTime).SetEase(lockedTweenInfo.ease));
		return sequence;
	}
}
