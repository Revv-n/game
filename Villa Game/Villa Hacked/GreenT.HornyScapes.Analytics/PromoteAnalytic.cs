using System;
using System.Collections.Generic;
using StripClub.Model.Cards;
using StripClub.UI.Collections.Promote;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Analytics;

public class PromoteAnalytic : AnalyticWindow<PromoteWindow>
{
	[Inject]
	private CardsCollection _cardsCollection;

	private readonly CompositeDisposable _onNewStream = new CompositeDisposable();

	private void OnEnable()
	{
		if (window.Card != null)
		{
			TrackPromote();
		}
	}

	private void OnDisable()
	{
		CompositeDisposable onNewStream = _onNewStream;
		if (onNewStream != null)
		{
			onNewStream.Clear();
		}
	}

	private void TrackPromote()
	{
		CompositeDisposable onNewStream = _onNewStream;
		if (onNewStream != null)
		{
			onNewStream.Clear();
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.Skip<int>((IObservable<int>)_cardsCollection.GetPromoteOrDefault(window.Card).Level, 1), (Action<int>)SendPromoteEvent), (ICollection<IDisposable>)_onNewStream);
	}

	private void SendPromoteEvent(int level)
	{
		PromoteAmplitudeEvent promoteAmplitudeEvent = new PromoteAmplitudeEvent(window.Card, level);
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)promoteAmplitudeEvent);
	}
}
