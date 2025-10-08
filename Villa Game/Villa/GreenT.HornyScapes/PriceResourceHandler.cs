using System;
using GreenT.Types;
using StripClub.Model;

namespace GreenT.HornyScapes;

public static class PriceResourceHandler
{
	private const string SOFT_SOURCE = "soft_money";

	private const string HARD_SOURCE = "hard_money";

	private const string PRESTIGE_SOURCE = "prestige_money";

	private const string EVENT_SOURCE = "event_money";

	private const string EVENT_XP_SOURCE = "event_xp_money";

	private const string NUTAKU_SOURCE = "nutaku";

	private const string REAL_SOURCE = "real";

	private const string MINIEVENT_SOURCE = "minievent_money";

	private const string BP_POINTS_SOURCE = "bp_points";

	private const string JEWELS_SOURCE = "jewels";

	private const string CONTTRACTS_SOURCE = "contracts";

	private const string EROLABS_ECOIN_SOURCE = "ecoin";

	public static (CurrencyType, CompositeIdentificator) ParsePriceSourse(string origin)
	{
		if (string.IsNullOrEmpty(origin))
		{
			return (CurrencyType.Soft, new CompositeIdentificator(default(int)));
		}
		string[] array = origin.Split(':');
		int[] array2 = null;
		return new ValueTuple<CurrencyType, CompositeIdentificator>(item2: new CompositeIdentificator((array.Length != 2) ? new int[1] : Array.ConvertAll(array[1].Split(','), int.Parse)), item1: GetCurrencyType(array[0]));
	}

	private static CurrencyType GetCurrencyType(string key)
	{
		return key switch
		{
			"soft_money" => CurrencyType.Soft, 
			"hard_money" => CurrencyType.Hard, 
			"prestige_money" => CurrencyType.Star, 
			"event_money" => CurrencyType.Event, 
			"event_xp_money" => CurrencyType.EventXP, 
			"real" => CurrencyType.Real, 
			"minievent_money" => CurrencyType.MiniEvent, 
			"bp_points" => CurrencyType.BP, 
			"jewels" => CurrencyType.Jewel, 
			"contracts" => CurrencyType.Contracts, 
			_ => CurrencyType.Soft, 
		};
	}
}
