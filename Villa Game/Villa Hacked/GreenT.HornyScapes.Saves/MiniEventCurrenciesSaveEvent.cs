using System;
using StripClub.Model;
using StripClub.Model.Data;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class MiniEventCurrenciesSaveEvent : SaveEvent
{
	private const CurrencyType _currencyType = CurrencyType.MiniEvent;

	private ICurrencyProcessor _currencyProcessor;

	private Currencies _mainBalance;

	[Inject]
	public void Init(Currencies mainBalance, ICurrencyProcessor currencyProcessor)
	{
		_mainBalance = mainBalance;
		_currencyProcessor = currencyProcessor;
	}

	public override void Track()
	{
		Initialize();
	}

	private void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.Skip<int>(Observable.SelectMany<(CurrencyType, SimpleCurrency), int>(Observable.Where<(CurrencyType, SimpleCurrency)>(_mainBalance.OnNewCurrency, (Func<(CurrencyType, SimpleCurrency), bool>)(((CurrencyType, SimpleCurrency) tuple) => tuple.Item1 == CurrencyType.MiniEvent)), (Func<(CurrencyType, SimpleCurrency), IObservable<int>>)(((CurrencyType, SimpleCurrency) tuple) => (IObservable<int>)_currencyProcessor.GetCountReactiveProperty(CurrencyType.MiniEvent, tuple.Item2.Identificator))), 1), (Action<int>)delegate
		{
			Save();
		}), (Component)this);
	}
}
