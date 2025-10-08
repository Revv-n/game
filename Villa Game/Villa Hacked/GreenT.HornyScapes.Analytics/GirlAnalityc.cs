using System;
using System.Collections.Generic;
using StripClub.Model.Cards;
using UniRx;

namespace GreenT.HornyScapes.Analytics;

public class GirlAnalityc : BaseEntityAnalytic<ICard>
{
	private const string ANALYTIC_LEVEL_UP_EVENT = "girl_level";

	private const string ANALYTIC_EVENT = "girl_in_collection";

	private CardsCollection cardsCollection;

	public GirlAnalityc(IAmplitudeSender<AmplitudeEvent> amplitude, CardsCollection cardsCollection)
		: base(amplitude)
	{
		this.cardsCollection = cardsCollection;
	}

	public override void Track()
	{
		onNewStream.Clear();
		TrackCardsOnUnlocking();
	}

	private void TrackPromote()
	{
		IObservable<IPromote> observable = Observable.Select<ICard, IPromote>(cardsCollection.OnCardUnlock, (Func<ICard, IPromote>)cardsCollection.GetPromoteOrDefault);
		IObservable<int> observable2 = Observable.Share<int>(Observable.SelectMany<IPromote, int>(Observable.Merge<IPromote>(Observable.ToObservable<IPromote>((IEnumerable<IPromote>)cardsCollection.Promote.Values), new IObservable<IPromote>[1] { observable }), (Func<IPromote, IObservable<int>>)((IPromote _promote) => Observable.Skip<int>((IObservable<int>)_promote.Level, 1))));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(ICard, int)>(Observable.CombineLatest<ICard, int, (ICard, int)>(Observable.Share<ICard>(Observable.ToObservable<ICard>(cardsCollection.Collection)), observable2, (Func<ICard, int, (ICard, int)>)((ICard _card, int _promoteLvlUp) => (_card: _card, _promoteLvlUp: _promoteLvlUp))), (Action<(ICard, int)>)SendPromoteEvent), (ICollection<IDisposable>)onNewStream);
	}

	private void TrackCardsOnUnlocking()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<ICard>(cardsCollection.OnCardUnlock, (Action<ICard>)SendEventByPass), (ICollection<IDisposable>)onNewStream);
	}

	public void SendPromoteEvent((ICard card, int level) group)
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("girl_level");
		((AnalyticsEvent)amplitudeEvent).AddEventParams(group.card.ID.ToString(), (object)group.level);
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent(amplitudeEvent);
	}

	public void SendPromoteEvent(int level)
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("girl_level");
		((AnalyticsEvent)amplitudeEvent).AddEventParams("girl_level", (object)level);
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent(amplitudeEvent);
	}

	public override void SendEventByPass(ICard tuple)
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("girl_in_collection");
		((AnalyticsEvent)amplitudeEvent).AddEventParams("girl_in_collection", (object)tuple.ID);
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent(amplitudeEvent);
	}
}
