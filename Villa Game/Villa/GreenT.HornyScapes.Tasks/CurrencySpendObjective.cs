using System;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class CurrencySpendObjective : CurrencyObjective
{
	public CurrencySpendObjective(Func<Sprite> iconProvider, CurrencyType currencyType, SavableObjectiveData data, ICurrencyProcessor currencyProcessor, int[] identificators)
		: base(iconProvider, currencyType, data, currencyProcessor, identificators)
	{
	}
}
