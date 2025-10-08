using System;
using GreenT.HornyScapes.BattlePassSpace;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class GetBPLevelsObjective : BattlePassObjective
{
	public GetBPLevelsObjective(Func<Sprite> iconProvider, SavableObjectiveData data, BattlePassProvider battlePassProvider)
		: base(iconProvider, data, battlePassProvider)
	{
	}

	public override void Track()
	{
		_battlePassStream?.Dispose();
		_battlePassStream = _battlePassProvider.CalendarChangeProperty.Value.battlePass.Data.LevelInfo.LevelProperty.Skip(1).Subscribe(delegate(int value)
		{
			AddProgress(value);
		});
	}
}
