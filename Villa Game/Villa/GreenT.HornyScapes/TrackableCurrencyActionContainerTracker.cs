using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes;

public class TrackableCurrencyActionContainerTracker
{
	private readonly Subject<ITrackableCurrencyContainer> _onNewContainer;

	private List<ITrackableCurrencyContainer> _trackableCurrencyActionContainers;

	public TrackableCurrencyActionContainerTracker()
	{
		_trackableCurrencyActionContainers = new List<ITrackableCurrencyContainer>();
		_onNewContainer = new Subject<ITrackableCurrencyContainer>();
	}

	public void OnNewContainer(ITrackableCurrencyContainer trackableCurrencyContainer)
	{
		_trackableCurrencyActionContainers.Add(trackableCurrencyContainer);
		_onNewContainer.OnNext(trackableCurrencyContainer);
	}

	public IObservable<int> GetAnySpendStream(CurrencyType currencyType)
	{
		IObservable<int> first = _trackableCurrencyActionContainers.Where((ITrackableCurrencyContainer container) => container.GetCurrencyType() == currencyType).ToObservable().SelectMany((ITrackableCurrencyContainer container) => container.OnSpend());
		IObservable<int> observable = _onNewContainer.Where((ITrackableCurrencyContainer container) => container.GetCurrencyType() == currencyType).SelectMany((ITrackableCurrencyContainer container) => container.OnSpend());
		return first.Merge(observable);
	}

	public IObservable<int> GetAnyAddStream(CurrencyType currencyType)
	{
		IObservable<int> first = _trackableCurrencyActionContainers.Where((ITrackableCurrencyContainer container) => container.GetCurrencyType() == currencyType).ToObservable().SelectMany((ITrackableCurrencyContainer container) => container.OnAdd());
		IObservable<int> observable = _onNewContainer.Where((ITrackableCurrencyContainer container) => container.GetCurrencyType() == currencyType).SelectMany((ITrackableCurrencyContainer container) => container.OnAdd());
		return first.Merge(observable);
	}
}
