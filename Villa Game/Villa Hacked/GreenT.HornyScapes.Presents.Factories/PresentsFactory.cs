using System;
using GreenT.HornyScapes.Presents.Models;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Presents.Factories;

public class PresentsFactory : IFactory<PresentsMapper, Present>, IFactory
{
	public Present Create(PresentsMapper mapper)
	{
		if (!TryGetCurrencyType(mapper.id, out var currencyType))
		{
			return null;
		}
		return new Present(mapper.id, mapper.points_per_item, mapper.longtap_stage_time, mapper.longtap_speed, currencyType);
	}

	private bool TryGetCurrencyType(string presentId, out CurrencyType currencyType)
	{
		currencyType = CurrencyType.None;
		if (!int.TryParse(presentId.Replace("present_", ""), out var result))
		{
			return false;
		}
		currencyType = result switch
		{
			1 => CurrencyType.Present1, 
			2 => CurrencyType.Present2, 
			3 => CurrencyType.Present3, 
			4 => CurrencyType.Present4, 
			_ => throw new Exception("Invalid present id (" + presentId + ")"), 
		};
		return true;
	}
}
