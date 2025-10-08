using System;
using StripClub.Model.Cards;
using UniRx;
using Zenject;

namespace GreenT.Bonus;

public class BonusController : IInitializable
{
	private readonly CardsCollection cardsCollection;

	public BonusController(CardsCollection cardsCollection)
	{
		this.cardsCollection = cardsCollection;
	}

	public void Initialize()
	{
		ObservableExtensions.Subscribe<ICard>(cardsCollection.OnCardUnlock, (Action<ICard>)delegate(ICard _card)
		{
			_card.Bonus.Apply();
		});
	}
}
