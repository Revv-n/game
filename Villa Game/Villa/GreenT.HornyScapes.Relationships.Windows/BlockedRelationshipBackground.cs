using System;
using System.Collections.Generic;
using DG.Tweening;
using GreenT.HornyScapes.Relationships.Views;
using Merge.Meta.RoomObjects;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Relationships.Windows;

public sealed class BlockedRelationshipBackground : MonoBehaviour
{
	private sealed class RelationshipInfo
	{
		public float Width;

		public float AnimatedWidth;

		public bool IsNeedAnimate;

		public CompositeDisposable Disposables;

		public RelationshipInfo(float initialOffset)
		{
			Width = initialOffset;
			AnimatedWidth = 0f;
			IsNeedAnimate = false;
			Disposables = new CompositeDisposable();
		}
	}

	[SerializeField]
	private RectTransform _rectTransform;

	[SerializeField]
	private CanvasGroup _blockSeparator;

	[SerializeField]
	private float _offsetX;

	[SerializeField]
	private string _blockedKey;

	[SerializeField]
	private float _startDelay = 0.35f;

	[SerializeField]
	private float _unblockDelay = 0.35f;

	[SerializeField]
	private float _unblockDuration = 0.25f;

	[SerializeField]
	private float _blockSeparatorDuration = 0.25f;

	private readonly Dictionary<int, RelationshipInfo> _infos = new Dictionary<int, RelationshipInfo>();

	public void AddReward(int relationshipId, BaseRewardView rewardView)
	{
		float rewardWidth = rewardView.RectTransform.rect.width;
		ReactiveProperty<EntityStatus> state = rewardView.Source.Rewards[0].State;
		if (!_infos.ContainsKey(relationshipId))
		{
			_infos[relationshipId] = new RelationshipInfo(_offsetX);
			_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _offsetX);
			_rectTransform.anchoredPosition = Vector2.zero;
			CheckLockSeparator(relationshipId);
		}
		RelationshipInfo relationshipInfo = _infos[relationshipId];
		if (state.Value == EntityStatus.Blocked)
		{
			AddWidth(relationshipId, rewardWidth);
		}
		IDisposable subscription = null;
		subscription = state.DistinctUntilChanged().Pairwise().Subscribe(delegate(Pair<EntityStatus> pair)
		{
			EntityStatus previous = pair.Previous;
			if (pair.Current == EntityStatus.Blocked)
			{
				AddWidth(relationshipId, rewardWidth);
			}
			else if (previous == EntityStatus.Blocked)
			{
				if (relationshipInfo.IsNeedAnimate)
				{
					relationshipInfo.AnimatedWidth += rewardWidth;
				}
				else
				{
					AddWidth(relationshipId, 0f - rewardWidth);
				}
				subscription.Dispose();
			}
		});
		relationshipInfo.Disposables.Add(subscription);
	}

	public void SetNeedAnimate(int relationshipId)
	{
		if (_infos.ContainsKey(relationshipId))
		{
			_infos[relationshipId].IsNeedAnimate = true;
		}
	}

	public void TryStartAnimation(int relationshipId)
	{
		if (!_infos.ContainsKey(relationshipId))
		{
			return;
		}
		RelationshipInfo relationshipInfo = _infos[relationshipId];
		if (!relationshipInfo.IsNeedAnimate)
		{
			return;
		}
		float num = 0f - relationshipInfo.AnimatedWidth;
		relationshipInfo.Width += num;
		if (!Mathf.Approximately(relationshipInfo.Width, _rectTransform.sizeDelta.x))
		{
			DOTween.Sequence().AppendInterval(_startDelay).Append(_blockSeparator.DOFade(0f, _blockSeparatorDuration))
				.AppendInterval(_unblockDelay)
				.Append(_rectTransform.DOSizeDelta(new Vector2(relationshipInfo.Width, _rectTransform.sizeDelta.y), _unblockDuration))
				.Append(_blockSeparator.DOFade((0f < relationshipInfo.Width) ? 1f : 0f, _blockSeparatorDuration))
				.OnComplete(delegate
				{
					CheckLockSeparator(relationshipId);
				});
		}
		relationshipInfo.IsNeedAnimate = false;
		relationshipInfo.AnimatedWidth = 0f;
	}

	public void Clear(int relationshipId)
	{
		if (_infos.ContainsKey(relationshipId))
		{
			RelationshipInfo relationshipInfo = _infos[relationshipId];
			relationshipInfo.Disposables.Clear();
			relationshipInfo.Width = _offsetX;
			relationshipInfo.AnimatedWidth = 0f;
			relationshipInfo.IsNeedAnimate = false;
			_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _offsetX);
			_rectTransform.anchoredPosition = Vector2.zero;
			CheckLockSeparator(relationshipId);
			_infos.Remove(relationshipId);
		}
	}

	private void AddWidth(int relationshipId, float width)
	{
		RelationshipInfo relationshipInfo = _infos[relationshipId];
		relationshipInfo.Width += width;
		_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, relationshipInfo.Width);
		CheckLockSeparator(relationshipId);
	}

	private void CheckLockSeparator(int relationshipId)
	{
		RelationshipInfo relationshipInfo = _infos[relationshipId];
		_blockSeparator.alpha = ((0f < relationshipInfo.Width) ? 1f : 0f);
	}
}
