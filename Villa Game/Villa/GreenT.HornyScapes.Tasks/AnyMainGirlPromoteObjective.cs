using System;
using GreenT.HornyScapes.Card;
using GreenT.Types;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class AnyMainGirlPromoteObjective : AnyGirlPromoteObjective
{
	public AnyMainGirlPromoteObjective(Func<Sprite> iconProvider, SavableObjectiveData data, CardsCollectionTracker cardsCollectionTracker)
		: base(iconProvider, data, cardsCollectionTracker, ContentType.Event)
	{
	}

	public override void Track()
	{
		anyPromoteStream?.Dispose();
		anyPromoteStream = (from _card in cardsCollectionTracker.GetAnyPromoteStream()
			where _card.ContentType != contentType
			select _card).Subscribe(delegate
		{
			AddProgress();
		});
	}
}
