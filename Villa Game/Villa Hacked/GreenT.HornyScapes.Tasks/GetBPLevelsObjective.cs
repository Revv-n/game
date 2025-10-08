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
		_battlePassStream = ObservableExtensions.Subscribe<int>(Observable.Skip<int>((IObservable<int>)_battlePassProvider.CalendarChangeProperty.Value.Item2.Data.LevelInfo.LevelProperty, 1), (Action<int>)delegate(int value)
		{
			AddProgress(value);
		});
	}
}
