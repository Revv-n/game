using System;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class ConcreteCurrencyAddObjective : CurrencyAddObjective
{
	public ConcreteCurrencyAddObjective(Func<Sprite> iconProvider, CurrencyType currencyType, SavableObjectiveData data, ICurrencyProcessor currencyProcessor, int[] identificators)
		: base(iconProvider, currencyType, data, currencyProcessor, identificators)
	{
	}

	public override void Track()
	{
		base.Track();
		_currencyStream?.Dispose();
		_currencyStream = _currencyProcessor.GetAddStream(_currencyType, AddProgress, _compositeIdentificator);
	}
}
