using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Relationships.Views;
using Merge.Meta.RoomObjects;
using UniRx;

namespace GreenT.HornyScapes.Relationships.Services;

public sealed class BlockLevelTracker
{
	private sealed class TrackerData
	{
		public bool IsNeedAnimate;

		public readonly List<IDisposable> Subscriptions = new List<IDisposable>(16);

		public readonly List<BaseRewardView> AllRewards = new List<BaseRewardView>(32);

		public readonly List<BaseRewardView> BlockedRewards = new List<BaseRewardView>(16);

		public readonly List<BaseRewardView> UnblockedRewards = new List<BaseRewardView>(8);

		public readonly List<BaseRewardView> DeletedRewards = new List<BaseRewardView>(16);

		public readonly Dictionary<BaseRewardView, IDisposable> RewardSubscriptions = new Dictionary<BaseRewardView, IDisposable>(16);
	}

	private readonly Dictionary<int, TrackerData> _trackers = new Dictionary<int, TrackerData>(4);

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	private readonly float _delay = 0.75f;

	public void AddReward(int relationshipId, BaseRewardView rewardView)
	{
		List<BaseRewardView> allRewards = GetOrCreateTracker(relationshipId).AllRewards;
		if (!allRewards.Contains(rewardView))
		{
			allRewards.Add(rewardView);
		}
		allRewards.Sort(CompareId);
	}

	public void AddBlockedReward(int relationshipId, BaseRewardView rewardView)
	{
		TrackerData tracker = GetOrCreateTracker(relationshipId);
		ReactiveProperty<EntityStatus> state = rewardView.Source.Rewards[0].State;
		if (state.Value != EntityStatus.Blocked)
		{
			return;
		}
		IObservable<EntityStatus> observable = Observable.DistinctUntilChanged<EntityStatus>((IObservable<EntityStatus>)state);
		IDisposable subscription = null;
		subscription = ObservableExtensions.Subscribe<EntityStatus>(observable, (Action<EntityStatus>)delegate(EntityStatus status)
		{
			List<BaseRewardView> blockedRewards2 = tracker.BlockedRewards;
			if (status == EntityStatus.Blocked)
			{
				if (!blockedRewards2.Contains(rewardView))
				{
					blockedRewards2.Add(rewardView);
				}
			}
			else
			{
				rewardView.SetActiveBlockedLevel(isActive: false);
				if (blockedRewards2.Remove(rewardView))
				{
					subscription.Dispose();
					tracker.RewardSubscriptions.Remove(rewardView);
				}
				CheckUnblockedRewards(tracker, rewardView);
				List<BaseRewardView> unblockedRewards = tracker.UnblockedRewards;
				if (!unblockedRewards.Contains(rewardView))
				{
					unblockedRewards.Add(rewardView);
				}
				tracker.IsNeedAnimate = true;
			}
		});
		tracker.Subscriptions.Add(subscription);
		tracker.RewardSubscriptions[rewardView] = subscription;
		if (state.Value == EntityStatus.Blocked)
		{
			List<BaseRewardView> blockedRewards = tracker.BlockedRewards;
			if (!blockedRewards.Contains(rewardView))
			{
				blockedRewards.Add(rewardView);
			}
		}
	}

	public void TryPrepareAnimation(int relationshipId)
	{
		if (!_trackers.TryGetValue(relationshipId, out var value))
		{
			return;
		}
		if (!value.IsNeedAnimate)
		{
			UpdateBlockedActives(value);
			return;
		}
		List<BaseRewardView> unblockedRewards = value.UnblockedRewards;
		if (0 < unblockedRewards.Count)
		{
			BaseRewardView baseRewardView = unblockedRewards[0];
			baseRewardView.SetActiveBlockedLevel(isActive: true);
			CheckUnblockedRewards(value, baseRewardView);
			PrepareAnimateUnblockedRewards(value);
		}
	}

	public void TryStartAnimation(int relationshipId)
	{
		if (_trackers.TryGetValue(relationshipId, out var tracker) && tracker.IsNeedAnimate)
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.Timer(TimeSpan.FromSeconds(_delay)), (Action<long>)delegate
			{
				StartAnimation(tracker);
			}), (ICollection<IDisposable>)_disposables);
		}
		void StartAnimation(TrackerData tracker)
		{
			if (0 < tracker.UnblockedRewards.Count)
			{
				BaseRewardView rewardView = tracker.UnblockedRewards[0];
				AnimateUnblockedRewards(relationshipId, tracker, rewardView);
				UpdateBlockedActives(tracker);
			}
			tracker.DeletedRewards.Clear();
			tracker.UnblockedRewards.Clear();
			tracker.IsNeedAnimate = false;
		}
	}

	public void Clear()
	{
		foreach (TrackerData value in _trackers.Values)
		{
			foreach (IDisposable subscription in value.Subscriptions)
			{
				subscription.Dispose();
			}
			value.Subscriptions.Clear();
			value.AllRewards.Clear();
			value.BlockedRewards.Clear();
			value.UnblockedRewards.Clear();
			value.DeletedRewards.Clear();
			value.RewardSubscriptions.Clear();
		}
	}

	private void UpdateBlockedActives(TrackerData tracker)
	{
		List<BaseRewardView> blockedRewards = tracker.BlockedRewards;
		if (blockedRewards.Count == 0)
		{
			foreach (BaseRewardView deletedReward in tracker.DeletedRewards)
			{
				deletedReward.SetActiveBlockedLevel(isActive: false);
			}
			return;
		}
		int minLevel = blockedRewards.Min((BaseRewardView rewardView) => rewardView.PromoteLevel);
		BaseRewardView baseRewardView = blockedRewards.First((BaseRewardView rewardView) => rewardView.PromoteLevel == minLevel);
		foreach (BaseRewardView allReward in tracker.AllRewards)
		{
			allReward.SetActiveBlockedLevel(allReward == baseRewardView);
		}
	}

	private void CheckUnblockedRewards(TrackerData tracker, BaseRewardView rewardView)
	{
		if (!tracker.IsNeedAnimate)
		{
			tracker.DeletedRewards.Clear();
		}
		tracker.AllRewards.Sort(CompareId);
		List<BaseRewardView> blockedRewards = tracker.BlockedRewards;
		int num = tracker.AllRewards.IndexOf(rewardView);
		int num2 = blockedRewards.IndexOf(rewardView);
		BaseRewardView baseRewardView;
		if (num2 + 1 >= blockedRewards.Count)
		{
			List<BaseRewardView> allRewards = tracker.AllRewards;
			baseRewardView = allRewards[allRewards.Count - 1];
		}
		else
		{
			baseRewardView = blockedRewards[num2 + 1];
		}
		BaseRewardView baseRewardView2 = baseRewardView;
		int num3 = tracker.AllRewards.IndexOf(baseRewardView2);
		if (num < 0 || num3 < 0)
		{
			return;
		}
		List<BaseRewardView> allRewards2 = tracker.AllRewards;
		List<BaseRewardView> deletedRewards = tracker.DeletedRewards;
		for (int i = num; i < num3; i++)
		{
			BaseRewardView item = allRewards2[i];
			if (!deletedRewards.Contains(item))
			{
				deletedRewards.Add(item);
			}
		}
		for (int num4 = num3 - 1; num4 >= num; num4--)
		{
			allRewards2.RemoveAt(num4);
		}
		if (baseRewardView2 == allRewards2[allRewards2.Count - 1])
		{
			if (!deletedRewards.Contains(baseRewardView2))
			{
				deletedRewards.Add(baseRewardView2);
			}
			allRewards2.Remove(baseRewardView2);
		}
	}

	private void PrepareAnimateUnblockedRewards(TrackerData tracker)
	{
		foreach (BaseRewardView deletedReward in tracker.DeletedRewards)
		{
			deletedReward.PrepareAnimateUnblockedRewards();
		}
		foreach (BaseRewardView blockedReward in tracker.BlockedRewards)
		{
			blockedReward.SetActiveBlockedLevel(isActive: false);
		}
	}

	private void AnimateUnblockedRewards(int relationshipId, TrackerData tracker, BaseRewardView rewardView)
	{
		List<BaseRewardView> blockedRewards = tracker.BlockedRewards;
		int num = blockedRewards.IndexOf(rewardView);
		BaseRewardView nextBlockedRewardView = ((num + 1 < blockedRewards.Count) ? blockedRewards[num + 1] : rewardView);
		List<BaseRewardView> deletedRewards = tracker.DeletedRewards;
		deletedRewards.Sort(CompareId);
		for (int i = 0; i < deletedRewards.Count; i++)
		{
			deletedRewards[i].AnimateUnblockedLevel(i, nextBlockedRewardView);
		}
	}

	private TrackerData GetOrCreateTracker(int relationshipId)
	{
		if (!_trackers.TryGetValue(relationshipId, out var value))
		{
			value = new TrackerData();
			_trackers[relationshipId] = value;
		}
		return value;
	}

	private static int CompareId(BaseRewardView first, BaseRewardView second)
	{
		return first.Source.Id.CompareTo(second.Source.Id);
	}
}
