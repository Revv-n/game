using System;
using GreenT.Types;
using StripClub.Model;

namespace GreenT.HornyScapes.Lootboxes;

[Serializable]
public class CurrencySelector : Selector
{
	public CurrencyType Currency { get; }

	public CompositeIdentificator Identificator { get; }

	public CurrencySelector(CurrencyType currency, CompositeIdentificator identificator)
	{
		Currency = currency;
		Identificator = identificator;
	}
}
