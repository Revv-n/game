using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Events;
using StripClub.Model;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.BattlePassSpace;

public class LevelRewardContainer : IInitializable, IDisposable
{
	public readonly IEnumerable<int> Levels;

	public readonly IEnumerable<RewardWithManyConditions> Rewards;

	public readonly ILookup<int, RewardWithManyConditions> RewardLookup;

	private readonly Subject<RewardWithManyConditions> _onRewardUpdate = new Subject<RewardWithManyConditions>();

	private readonly CompositeDisposable _updateStream = new CompositeDisposable();

	public LevelRewardContainer(IEnumerable<(int, RewardWithManyConditions)> rewards)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		RewardLookup = rewards.ToLookup(((int, RewardWithManyConditions) pair) => pair.Item1, ((int, RewardWithManyConditions) pair) => pair.Item2);
		Levels = RewardLookup.Select((IGrouping<int, RewardWithManyConditions> tuple) => tuple.Key);
		Rewards = GetRewards(RewardLookup);
	}

	public void Initialize()
	{
		CompositeDisposable updateStream = _updateStream;
		if (updateStream != null)
		{
			updateStream.Clear();
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<RewardWithManyConditions>(Observable.Merge<RewardWithManyConditions>(Rewards.Select((RewardWithManyConditions _reward) => _reward.OnUpdate)), (Action<RewardWithManyConditions>)_onRewardUpdate.OnNext), (ICollection<IDisposable>)_updateStream);
	}

	public IEnumerable<RewardWithManyConditions> GetRewardsByLinkedContent<T>() where T : LinkedContent
	{
		return Rewards.Where((RewardWithManyConditions reward) => reward.Content is T);
	}

	private IEnumerable<RewardWithManyConditions> GetRewards(ILookup<int, RewardWithManyConditions> rewards)
	{
		return rewards.Aggregate(new List<RewardWithManyConditions>(), delegate(List<RewardWithManyConditions> current, IGrouping<int, RewardWithManyConditions> tuple)
		{
			current.AddRange(tuple);
			return current;
		});
	}

	public int GetMaxLevel()
	{
		return Levels.Max();
	}

	public int GetLastUnlockedLevel()
	{
		return (from value in RewardLookup.SelectMany((IGrouping<int, RewardWithManyConditions> @group) => from reward in @group
				where reward.IsComplete || reward.IsRewarded
				select reward into _
				select @group.Key)
			orderby value
			select value).LastOrDefault();
	}

	public IEnumerable<RewardWithManyConditions> GetRewardsForLevel(int level)
	{
		return RewardLookup.Where((IGrouping<int, RewardWithManyConditions> data) => data.Key == level).SelectMany((IGrouping<int, RewardWithManyConditions> key) => key.AsEnumerable());
	}

	public IObservable<RewardWithManyConditions> OnRewardUpdate()
	{
		return Observable.AsObservable<RewardWithManyConditions>((IObservable<RewardWithManyConditions>)_onRewardUpdate);
	}

	public void Dispose()
	{
		_onRewardUpdate.OnCompleted();
		_onRewardUpdate.Dispose();
		_updateStream.Dispose();
	}
}
