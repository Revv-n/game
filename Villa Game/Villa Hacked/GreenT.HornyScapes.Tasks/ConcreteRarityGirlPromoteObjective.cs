using System;
using GreenT.HornyScapes.Card;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class ConcreteRarityGirlPromoteObjective : AnyMainGirlPromoteObjective
{
	public readonly Rarity Rarity;

	public ConcreteRarityGirlPromoteObjective(Func<Sprite> iconProvider, SavableObjectiveData data, CardsCollectionTracker cardsCollectionTracker, Rarity rarity)
		: base(iconProvider, data, cardsCollectionTracker)
	{
		Rarity = rarity;
	}

	public override void Track()
	{
		anyPromoteStream?.Dispose();
		anyPromoteStream = ObservableExtensions.Subscribe<ICard>(Observable.Where<ICard>(cardsCollectionTracker.GetAnyPromoteStream(), (Func<ICard, bool>)((ICard _card) => _card.ContentType != contentType && _card.Rarity == Rarity)), (Action<ICard>)delegate
		{
			AddProgress();
		});
	}
}
