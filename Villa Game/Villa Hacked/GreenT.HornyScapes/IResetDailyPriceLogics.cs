using System;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes;

public interface IResetDailyPriceLogics
{
	bool IsFree { get; }

	IObservable<Price<int>> OnPriceUpdate { get; }

	Price<int> Price { get; }
}
