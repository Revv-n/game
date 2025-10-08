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
		_anyGetStream = ObservableExtensions.Subscribe<(ICard, int)>(Observable.Where<(ICard, int)>(_cardsCollectionTracker.OnNewCardStream(), (Func<(ICard, int), bool>)(((ICard, int) value) => value.Item1.ContentType != ContentType.Event && value.Item1.Rarity == Rarity)), (Action<(ICard, int)>)delegate((ICard, int) value)
		{
			AddProgress(value.Item2);
		});
	}
}
