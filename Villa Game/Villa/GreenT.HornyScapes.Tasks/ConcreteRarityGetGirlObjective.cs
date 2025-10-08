using System;
using GreenT.HornyScapes.Card;
using GreenT.Types;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class ConcreteRarityGetGirlObjective : AnyGetGirlObjective
{
	public readonly Rarity Rarity;

	public ConcreteRarityGetGirlObjective(Func<Sprite> iconProvider, SavableObjectiveData data, CardsCollectionTracker cardsCollectionTracker, Rarity rarity)
		: base(iconProvider, data, cardsCollectionTracker)
	{
		Rarity = rarity;
	}

	public override void Track()
	{
		base.Track();
		_anyGetStream?.Dispose();
		_anyGetStream = (from value in _cardsCollectionTracker.OnNewCardStream()
			where value.Item1.ContentType != ContentType.Event && value.Item1.Rarity == Rarity
			select value).Subscribe(delegate((ICard, int) value)
		{
			AddProgress(value.Item2);
		});
	}
}
