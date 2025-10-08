using System;
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
		IObservable<IPromote> observable = cardsCollection.OnCardUnlock.Select(cardsCollection.GetPromoteOrDefault);
		IObservable<int> right = cardsCollection.Promote.Values.ToObservable().Merge(observable).SelectMany((IPromote _promote) => _promote.Level.Skip(1))
			.Share();
		cardsCollection.Collection.ToObservable().Share().CombineLatest(right, (ICard _card, int _promoteLvlUp) => (_card: _card, _promoteLvlUp: _promoteLvlUp))
			.Subscribe(SendPromoteEvent)
			.AddTo(onNewStream);
	}

	private void TrackCardsOnUnlocking()
	{
		cardsCollection.OnCardUnlock.Subscribe(SendEventByPass).AddTo(onNewStream);
	}

	public void SendPromoteEvent((ICard card, int level) group)
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("girl_level");
		amplitudeEvent.AddEventParams(group.card.ID.ToString(), group.level);
		amplitude.AddEvent(amplitudeEvent);
	}

	public void SendPromoteEvent(int level)
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("girl_level");
		amplitudeEvent.AddEventParams("girl_level", level);
		amplitude.AddEvent(amplitudeEvent);
	}

	public override void SendEventByPass(ICard tuple)
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("girl_in_collection");
		amplitudeEvent.AddEventParams("girl_in_collection", tuple.ID);
		amplitude.AddEvent(amplitudeEvent);
	}
}
