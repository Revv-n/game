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
		RewardLookup = rewards.ToLookup(((int, RewardWithManyConditions) pair) => pair.Item1, ((int, RewardWithManyConditions) pair) => pair.Item2);
		Levels = RewardLookup.Select((IGrouping<int, RewardWithManyConditions> tuple) => tuple.Key);
		Rewards = GetRewards(RewardLookup);
	}

	public void Initialize()
	{
		_updateStream?.Clear();
		Rewards.Select((RewardWithManyConditions _reward) => _reward.OnUpdate).Merge().Subscribe(_onRewardUpdate.OnNext)
			.AddTo(_updateStream);
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
		return _onRewardUpdate.AsObservable();
	}

	public void Dispose()
	{
		_onRewardUpdate.OnCompleted();
		_onRewardUpdate.Dispose();
		_updateStream.Dispose();
	}
}
