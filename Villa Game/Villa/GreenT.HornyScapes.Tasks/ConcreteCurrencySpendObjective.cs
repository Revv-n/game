using System;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class ConcreteCurrencySpendObjective : CurrencySpendObjective
{
	public ConcreteCurrencySpendObjective(Func<Sprite> iconProvider, CurrencyType currencyType, SavableObjectiveData data, ICurrencyProcessor currencyProcessor, int[] identificators)
		: base(iconProvider, currencyType, data, currencyProcessor, identificators)
	{
	}

	public override void Track()
	{
		base.Track();
		_currencyStream?.Dispose();
		_currencyStream = _currencyProcessor.GetSpendStream(_currencyType, AddProgress, _compositeIdentificator);
	}
}
