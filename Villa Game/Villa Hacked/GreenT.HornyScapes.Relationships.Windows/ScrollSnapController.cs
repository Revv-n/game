using System.Collections.Generic;
using DG.Tweening;
using GreenT.HornyScapes.Relationships.Views;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Relationships.Windows;

public class ScrollSnapController : MonoBehaviour
{
	[SerializeField]
	private ScrollRect _scroll;

	[SerializeField]
	private float _snapDuration = 0.5f;

	[SerializeField]
	private float _padding = 24f;

	private Tween _snapTween;

	private int _snapTarget;

	private readonly List<BaseRewardView> _rewardViews = new List<BaseRewardView>(64);

	public void Add(BaseRewardView rewardView)
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
			_snapTween = DOTweenModuleUI.DOHorizontalNormalizedPos(_scroll, endValue, _snapDuration);
		}
	}

	public void Clear()
	{
		_rewardViews.Clear();
	}

	private float CalculateSnapPosition()
	{
		RectTransform content = _scroll.content;
		float width = _scroll.viewport.rect.width;
		float num = 0f;
		for (int i = 0; i <= _snapTarget; i++)
		{
			num += _rewardViews[i].RectTransform.rect.width;
		}
		float num2 = num - width + _padding;
		float num3 = content.rect.width - width;
		return Mathf.Clamp01(num2 / num3);
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
		float num3 = 0f - x + width * 0.25f;
		float num4 = num3 + viewport.rect.width;
		if (num3 <= num)
		{
			return num2 <= num4;
		}
		return false;
	}
}
