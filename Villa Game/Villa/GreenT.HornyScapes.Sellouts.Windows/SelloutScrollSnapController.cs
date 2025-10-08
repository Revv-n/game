using System.Collections.Generic;
using DG.Tweening;
using GreenT.HornyScapes.Sellouts.Views;
using Merge.Meta.RoomObjects;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Sellouts.Windows;

public sealed class SelloutScrollSnapController : MonoBehaviour
{
	[SerializeField]
	private ScrollRect _scroll;

	[SerializeField]
	private float _snapDuration = 0.5f;

	[SerializeField]
	[Range(0f, 1f)]
	private float _leftOffset = 0.25f;

	private Tween _snapTween;

	private int _snapTarget;

	private readonly List<SelloutRewardView> _rewardViews = new List<SelloutRewardView>(64);

	public void Add(SelloutRewardView rewardView)
	{
		_rewardViews.Add(rewardView);
	}

	public void SnapTo(int target)
	{
		_snapTarget = Mathf.Clamp(target, 0, _rewardViews.Count - 1);
		if (!IsTargetVisible(_snapTarget))
		{
			if (_snapTween != null && _snapTween.IsPlaying())
			{
				_snapTween.Kill();
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(_scroll.content);
			float endValue = CalculateSnapPosition();
			_snapTween = _scroll.DOHorizontalNormalizedPos(endValue, _snapDuration);
		}
	}

	public void SnapToAppropriateReward()
	{
		int target = FindSnapIndex();
		SnapTo(target);
	}

	public void Clear()
	{
		_rewardViews.Clear();
	}

	private int FindSnapIndex()
	{
		int num = -1;
		for (int i = 0; i < _rewardViews.Count; i++)
		{
			switch (_rewardViews[i].GetState())
			{
			case EntityStatus.Complete:
				return i;
			case EntityStatus.Rewarded:
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			return 0;
		}
		return num;
	}

	private float CalculateSnapPosition()
	{
		RectTransform content = _scroll.content;
		float width = _scroll.viewport.rect.width;
		float width2 = content.rect.width;
		if (width2 <= width)
		{
			return 0f;
		}
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < _snapTarget; i++)
		{
			num2 += _rewardViews[i].RectTransform.rect.width;
		}
		float value = num - num2;
		float min = 0f - (width2 - width);
		float max = 0f;
		return (0f - Mathf.Clamp(value, min, max)) / (width2 - width);
	}

	private bool IsTargetVisible(int index)
	{
		RectTransform content = _scroll.content;
		RectTransform viewport = _scroll.viewport;
		RectTransform rectTransform = _rewardViews[index].RectTransform;
		float x = content.anchoredPosition.x;
		float num = 0f;
		for (int i = 0; i < index; i++)
		{
			num += _rewardViews[i].RectTransform.rect.width;
		}
		float width = rectTransform.rect.width;
		float num2 = num + width;
		float num3 = 0f - x - width;
		float num4 = num3 + viewport.rect.width * _leftOffset;
		if (num3 <= num)
		{
			return num2 <= num4;
		}
		return false;
	}
}
