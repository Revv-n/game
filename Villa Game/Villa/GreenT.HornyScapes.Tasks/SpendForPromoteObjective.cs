using System;
using GreenT.HornyScapes.Collections.Promote.UI;
using StripClub.Model;
using StripClub.Model.Data;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class SpendForPromoteObjective : GainObjective, IDisposable
{
	private readonly CurrencyType _currencyType;

	private readonly PromoteNotifier _promoteNotifier;

	private IDisposable _currencyStream;

	public CurrencyType CurrencyType => _currencyType;

	public SpendForPromoteObjective(Func<Sprite> iconProvider, CurrencyType currencyType, PromoteNotifier promoteNotifier, SavableObjectiveData data)
		: base(iconProvider, data)
	{
		_currencyType = currencyType;
		_promoteNotifier = promoteNotifier;
	}

	public override void Track()
	{
		base.Track();
		_currencyStream?.Dispose();
		_currencyStream = (from cost in _promoteNotifier.OnNotify
			where cost.Currency == _currencyType
			select cost.Amount).Subscribe(AddProgress);
	}

	public override void OnRewardTask()
	{
		base.OnRewardTask();
		_currencyStream?.Dispose();
	}

	public void Dispose()
	{
		_currencyStream?.Dispose();
	}

	protected virtual void AddProgress(int value)
	{
		Data.Progress += value;
		onUpdate.OnNext(this);
	}
}
