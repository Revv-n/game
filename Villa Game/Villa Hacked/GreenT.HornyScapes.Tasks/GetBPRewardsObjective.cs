using System;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using Merge.Meta.RoomObjects;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class GetBPRewardsObjective : BattlePassObjective
{
	public GetBPRewardsObjective(Func<Sprite> iconProvider, SavableObjectiveData data, BattlePassProvider battlePassProvider)
		: base(iconProvider, data, battlePassProvider)
	{
	}

	public override void Track()
	{
		_battlePassStream?.Dispose();
		_battlePassStream = ObservableExtensions.Subscribe<RewardWithManyConditions>(Observable.Where<RewardWithManyConditions>(_battlePassProvider.CalendarChangeProperty.Value.Item2.AllRewardContainer.OnRewardUpdate(), (Func<RewardWithManyConditions, bool>)((RewardWithManyConditions reward) => reward.State.Value == EntityStatus.Rewarded)), (Action<RewardWithManyConditions>)delegate
		{
			AddProgress();
		});
	}
}
