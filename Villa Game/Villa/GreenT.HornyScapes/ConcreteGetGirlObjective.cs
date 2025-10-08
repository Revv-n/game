using System;
using GreenT.HornyScapes.Card;
using GreenT.HornyScapes.Tasks;
using GreenT.Types;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes;

[Objective]
public class ConcreteGetGirlObjective : AnyGetGirlObjective
{
	public readonly int GirlId;

	public ConcreteGetGirlObjective(Func<Sprite> iconProvider, SavableObjectiveData data, CardsCollectionTracker cardsCollectionTracker, int girlId)
		: base(iconProvider, data, cardsCollectionTracker)
	{
		GirlId = girlId;
	}

	public override void Track()
	{
		_anyGetStream?.Dispose();
		_anyGetStream = (from value in _cardsCollectionTracker.OnNewCardStream()
			where value.Item1.ContentType != ContentType.Event && value.Item1.ID == GirlId
			select value).Subscribe(delegate((ICard, int) value)
		{
			AddProgress(value.Item2);
		});
	}
}
