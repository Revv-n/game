using System;
using GreenT.HornyScapes.Card;
using GreenT.Types;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public sealed class ConcreteGirlPromoteObjective : AnyGirlPromoteObjective
{
	public readonly int GirlID;

	public readonly int TargetLevel;

	public ConcreteGirlPromoteObjective(Func<Sprite> iconProvider, SavableObjectiveData data, CardsCollectionTracker cardsCollectionTracker, int girlId, int targetLevel, ContentType contentType)
		: base(iconProvider, data, cardsCollectionTracker, contentType)
	{
		GirlID = girlId;
		TargetLevel = targetLevel;
	}

	public override void Track()
	{
		SetProgress();
		anyPromoteStream?.Dispose();
		anyPromoteStream = (from value in cardsCollectionTracker.GetConcretePromoteStream()
			where value.Item1.ContentType == contentType && value.Item1.ID == GirlID && value.Item2 >= TargetLevel
			select value).Subscribe(delegate
		{
			AddProgress();
		});
	}

	private void SetProgress()
	{
		if (cardsCollectionTracker.TryGetGirlLevel(GirlID) >= TargetLevel)
		{
			AddProgress();
		}
	}
}
