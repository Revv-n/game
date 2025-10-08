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
		IObservable<int> observable = Observable.SelectMany<ITrackableCurrencyContainer, int>(Observable.ToObservable<ITrackableCurrencyContainer>(_trackableCurrencyActionContainers.Where((ITrackableCurrencyContainer container) => container.GetCurrencyType() == currencyType)), (Func<ITrackableCurrencyContainer, IObservable<int>>)((ITrackableCurrencyContainer container) => container.OnSpend()));
		IObservable<int> observable2 = Observable.SelectMany<ITrackableCurrencyContainer, int>(Observable.Where<ITrackableCurrencyContainer>((IObservable<ITrackableCurrencyContainer>)_onNewContainer, (Func<ITrackableCurrencyContainer, bool>)((ITrackableCurrencyContainer container) => container.GetCurrencyType() == currencyType)), (Func<ITrackableCurrencyContainer, IObservable<int>>)((ITrackableCurrencyContainer container) => container.OnSpend()));
		return Observable.Merge<int>(observable, new IObservable<int>[1] { observable2 });
	}

	public IObservable<int> GetAnyAddStream(CurrencyType currencyType)
	{
		IObservable<int> observable = Observable.SelectMany<ITrackableCurrencyContainer, int>(Observable.ToObservable<ITrackableCurrencyContainer>(_trackableCurrencyActionContainers.Where((ITrackableCurrencyContainer container) => container.GetCurrencyType() == currencyType)), (Func<ITrackableCurrencyContainer, IObservable<int>>)((ITrackableCurrencyContainer container) => container.OnAdd()));
		IObservable<int> observable2 = Observable.SelectMany<ITrackableCurrencyContainer, int>(Observable.Where<ITrackableCurrencyContainer>((IObservable<ITrackableCurrencyContainer>)_onNewContainer, (Func<ITrackableCurrencyContainer, bool>)((ITrackableCurrencyContainer container) => container.GetCurrencyType() == currencyType)), (Func<ITrackableCurrencyContainer, IObservable<int>>)((ITrackableCurrencyContainer container) => container.OnAdd()));
		return Observable.Merge<int>(observable, new IObservable<int>[1] { observable2 });
	}
}
