using System;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class CurrencySaveEvent : SaveEvent
{
	[SerializeField]
	private CurrencyType currency;

	private ICurrencyProcessor currencyProcessor;

	private GameStarter gameStarter;

	[Inject]
	public void Init(ICurrencyProcessor currencyProcessor, GameStarter gameStarter)
	{
		this.currencyProcessor = currencyProcessor;
		this.gameStarter = gameStarter;
	}

	public override void Track()
	{
		Initialize();
	}

	private void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.Skip<int>(Observable.ContinueWith<bool, int>(Observable.FirstOrDefault<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), (Func<bool, IObservable<int>>)((bool _) => (IObservable<int>)currencyProcessor.GetCountReactiveProperty(currency))), 1), (Action<int>)delegate
		{
			Save();
		}), (Component)this);
	}
}
