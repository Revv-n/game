using System;
using GreenT.HornyScapes.BattlePassSpace;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public abstract class BattlePassObjective : GainObjective
{
	protected readonly BattlePassProvider _battlePassProvider;

	protected IDisposable _battlePassStream;

	public BattlePassObjective(Func<Sprite> iconProvider, SavableObjectiveData data, BattlePassProvider battlePassProvider)
		: base(iconProvider, data)
	{
		_battlePassProvider = battlePassProvider;
	}

	public override void OnRewardTask()
	{
		base.OnRewardTask();
		_battlePassStream?.Dispose();
	}

	protected void AddProgress(int value)
	{
		Data.Progress += value;
		onUpdate.OnNext(this);
	}

	protected void AddProgress()
	{
		Data.Progress++;
		onUpdate.OnNext(this);
	}
}
