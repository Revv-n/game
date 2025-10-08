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
		_battlePassStream = (from reward in _battlePassProvider.CalendarChangeProperty.Value.battlePass.AllRewardContainer.OnRewardUpdate()
			where reward.State.Value == EntityStatus.Rewarded
			select reward).Subscribe(delegate
		{
			AddProgress();
		});
	}
}
