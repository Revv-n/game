using System;
using System.Collections.Generic;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class EnergySpendSaveEvent : SaveEvent
{
	[SerializeField]
	private CurrencyType currencyType;

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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.Where<int>(Observable.Scan<int>(Observable.Where<int>(Observable.Select<Pair<int>, int>(Observable.Pairwise<int>(Observable.ContinueWith<bool, int>(Observable.First<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool _isActive) => _isActive)), (Func<bool, IObservable<int>>)((bool _) => (IObservable<int>)currencyProcessor.GetCountReactiveProperty(currencyType)))), (Func<Pair<int>, int>)((Pair<int> _energy) => _energy.Previous - _energy.Current)), (Func<int, bool>)((int _diff) => _diff > 0)), (Func<int, int, int>)((int _previous, int _current) => _previous + _current)), (Func<int, bool>)((int _diff) => _diff % 10 == 0)), (Action<int>)delegate
		{
			Save();
		}), (ICollection<IDisposable>)saveStreams);
	}
}
