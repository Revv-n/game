using System;
using StripClub.Model;

namespace GreenT.HornyScapes;

public interface ITrackableCurrencyContainer : IDisposable
{
	CurrencyType GetCurrencyType();

	IObservable<int> OnSpend();

	IObservable<int> OnAdd();
}
