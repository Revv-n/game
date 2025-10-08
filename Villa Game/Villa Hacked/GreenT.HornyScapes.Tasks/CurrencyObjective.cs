using System;
using System.Linq;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public abstract class CurrencyObjective : GainObjective, IDisposable
{
	protected readonly CurrencyType _currencyType;

	protected readonly CompositeIdentificator _compositeIdentificator;

	protected readonly ICurrencyProcessor _currencyProcessor;

	protected IDisposable _currencyStream;

	public CurrencyType CurrencyType => _currencyType;

	public int CurrencyId => _compositeIdentificator[0];

	public CurrencyObjective(Func<Sprite> iconProvider, CurrencyType currencyType, SavableObjectiveData data, ICurrencyProcessor currencyProcessor, int[] identificators)
		: base(iconProvider, data)
	{
		_currencyType = currencyType;
		_currencyProcessor = currencyProcessor;
		if (identificators == null || !identificators.Any())
		{
			identificators = new int[1];
		}
		_compositeIdentificator = new CompositeIdentificator(identificators);
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
		onUpdate.OnNext((IObjective)this);
	}
}
