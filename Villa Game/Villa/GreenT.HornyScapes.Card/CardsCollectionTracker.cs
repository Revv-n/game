using System;
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
		IObservable<ICard> first = cardsCollection.Promote.Keys.ToObservable().SelectMany((Func<ICard, IObservable<ICard>>)OnCardAddSouls);
		IObservable<ICard> observable = cardsCollection.OnNewPromote.SelectMany((Func<ICard, IObservable<ICard>>)OnCardAddSouls);
		return first.Merge(observable);
		IObservable<ICard> OnCardAddSouls(ICard card)
		{
			return from _ in cardsCollection.Promote[card].Progress.Skip(1)
				select card;
		}
	}

	public IObservable<ICard> GetAnyPromoteStream()
	{
		IObservable<ICard> first = cardsCollection.Promote.Keys.ToObservable().SelectMany((Func<ICard, IObservable<ICard>>)OnLevelUpCard);
		IObservable<ICard> observable = cardsCollection.OnNewPromote.SelectMany((Func<ICard, IObservable<ICard>>)OnLevelUpCard);
		return first.Merge(observable);
		IObservable<ICard> OnLevelUpCard(ICard card)
		{
			return from _ in cardsCollection.Promote[card].Level.Skip(1)
				select card;
		}
	}

	public IObservable<(ICard, int)> GetConcretePromoteStream()
	{
		IObservable<(ICard, int)> first = cardsCollection.Promote.Keys.ToObservable().SelectMany((Func<ICard, IObservable<(ICard, int)>>)OnLevelUpCard);
		IObservable<(ICard, int)> observable = cardsCollection.OnNewPromote.SelectMany((Func<ICard, IObservable<(ICard, int)>>)OnLevelUpCard);
		return first.Merge(observable);
		IObservable<(ICard, int)> OnLevelUpCard(ICard card)
		{
			return from level in cardsCollection.Promote[card].Level.Skip(1)
				select (card: card, level: level);
		}
	}
}
