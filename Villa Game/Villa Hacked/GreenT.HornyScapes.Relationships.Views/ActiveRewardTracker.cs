using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Relationships.Models;
using Merge.Meta.RoomObjects;
using StripClub.UI;
using UniRx;

namespace GreenT.HornyScapes.Relationships.Views;

public class ActiveRewardTracker : MonoView<Relationship>, IDisposable
{
	private IDisposable _rewardTrackStream;

	private readonly Subject<IReadOnlyList<RewardWithManyConditions>> _activeRewardsChanged = new Subject<IReadOnlyList<RewardWithManyConditions>>();

	public IObservable<IReadOnlyList<RewardWithManyConditions>> ActiveRewardsChanged => Observable.AsObservable<IReadOnlyList<RewardWithManyConditions>>((IObservable<IReadOnlyList<RewardWithManyConditions>>)_activeRewardsChanged);

	public override void Set(Relationship source)
	{
		base.Set(source);
		_rewardTrackStream?.Dispose();
		EmitFirstInProgressReward();
		_rewardTrackStream = ObservableExtensions.Subscribe<EntityStatus>(Observable.Merge<EntityStatus>(Observable.SelectMany<IReadOnlyList<RewardWithManyConditions>, EntityStatus>(Observable.ToObservable<IReadOnlyList<RewardWithManyConditions>>((IEnumerable<IReadOnlyList<RewardWithManyConditions>>)source.Rewards), (Func<IReadOnlyList<RewardWithManyConditions>, IObservable<EntityStatus>>)((IReadOnlyList<RewardWithManyConditions> x) => (IObservable<EntityStatus>)x.FirstOrDefault().State)), Array.Empty<IObservable<EntityStatus>>()), (Action<EntityStatus>)delegate
		{
			EmitFirstInProgressReward();
		});
	}

	public void Dispose()
	{
		_rewardTrackStream?.Dispose();
		_activeRewardsChanged.Dispose();
	}

	private void OnDestroy()
	{
		Dispose();
	}

	private void EmitFirstInProgressReward()
	{
		IReadOnlyList<RewardWithManyConditions> readOnlyList = base.Source.Rewards.FirstOrDefault((IReadOnlyList<RewardWithManyConditions> x) => x.All((RewardWithManyConditions x) => x.State.Value == EntityStatus.InProgress));
		if (readOnlyList != null)
		{
			_activeRewardsChanged.OnNext(readOnlyList);
		}
	}
}
