using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Cards;
using UniRx;

namespace GreenT.HornyScapes.Card;

public class CardsCollectionTracker
{
	private readonly CardsCollection cardsCollection;

	public CardsCollectionTracker(CardsCollection cardsCollection)
	{
		this.cardsCollection = cardsCollection;
	}

	public int TryGetGirlLevel(int girlId)
	{
		ICard card2 = cardsCollection.Promote.Keys.FirstOrDefault((ICard card) => card.ID == girlId);
		if (card2 != null)
		{
			return cardsCollection.Promote[card2].Level.Value;
		}
		return 0;
	}

	public IObservable<(ICard, int)> OnNewCardStream()
	{
		return cardsCollection.OnNewSoulsCard;
	}

	public IObservable<ICard> GetAnyCardAddSoulsStream()
	{
		IObservable<ICard> observable = Observable.SelectMany<ICard, ICard>(Observable.ToObservable<ICard>((IEnumerable<ICard>)cardsCollection.Promote.Keys), (Func<ICard, IObservable<ICard>>)OnCardAddSouls);
		IObservable<ICard> observable2 = Observable.SelectMany<ICard, ICard>(cardsCollection.OnNewPromote, (Func<ICard, IObservable<ICard>>)OnCardAddSouls);
		return Observable.Merge<ICard>(observable, new IObservable<ICard>[1] { observable2 });
		IObservable<ICard> OnCardAddSouls(ICard card)
		{
			return Observable.Select<int, ICard>(Observable.Skip<int>((IObservable<int>)cardsCollection.Promote[card].Progress, 1), (Func<int, ICard>)((int _) => card));
		}
	}

	public IObservable<ICard> GetAnyPromoteStream()
	{
		IObservable<ICard> observable = Observable.SelectMany<ICard, ICard>(Observable.ToObservable<ICard>((IEnumerable<ICard>)cardsCollection.Promote.Keys), (Func<ICard, IObservable<ICard>>)OnLevelUpCard);
		IObservable<ICard> observable2 = Observable.SelectMany<ICard, ICard>(cardsCollection.OnNewPromote, (Func<ICard, IObservable<ICard>>)OnLevelUpCard);
		return Observable.Merge<ICard>(observable, new IObservable<ICard>[1] { observable2 });
		IObservable<ICard> OnLevelUpCard(ICard card)
		{
			return Observable.Select<int, ICard>(Observable.Skip<int>((IObservable<int>)cardsCollection.Promote[card].Level, 1), (Func<int, ICard>)((int _) => card));
		}
	}

	public IObservable<(ICard, int)> GetConcretePromoteStream()
	{
		IObservable<(ICard, int)> observable = Observable.SelectMany<ICard, (ICard, int)>(Observable.ToObservable<ICard>((IEnumerable<ICard>)cardsCollection.Promote.Keys), (Func<ICard, IObservable<(ICard, int)>>)OnLevelUpCard);
		IObservable<(ICard, int)> observable2 = Observable.SelectMany<ICard, (ICard, int)>(cardsCollection.OnNewPromote, (Func<ICard, IObservable<(ICard, int)>>)OnLevelUpCard);
		return Observable.Merge<(ICard, int)>(observable, new IObservable<(ICard, int)>[1] { observable2 });
		IObservable<(ICard, int)> OnLevelUpCard(ICard card)
		{
			return Observable.Select<int, (ICard, int)>(Observable.Skip<int>((IObservable<int>)cardsCollection.Promote[card].Level, 1), (Func<int, (ICard, int)>)((int level) => (card: card, level: level)));
		}
	}
}
