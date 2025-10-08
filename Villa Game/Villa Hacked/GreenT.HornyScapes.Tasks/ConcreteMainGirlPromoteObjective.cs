using System;
using GreenT.HornyScapes.Card;
using GreenT.Types;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public sealed class ConcreteMainGirlPromoteObjective : AnyGirlPromoteObjective
{
	public readonly int GirlID;

	public ConcreteMainGirlPromoteObjective(Func<Sprite> iconProvider, SavableObjectiveData data, CardsCollectionTracker cardsCollectionTracker, int girlId)
		: base(iconProvider, data, cardsCollectionTracker, ContentType.Event)
	{
		GirlID = girlId;
	}

	public override void Track()
	{
		SetProgress();
		anyPromoteStream?.Dispose();
		anyPromoteStream = ObservableExtensions.Subscribe<(ICard, int)>(Observable.Where<(ICard, int)>(cardsCollectionTracker.GetConcretePromoteStream(), (Func<(ICard, int), bool>)(((ICard, int) value) => value.Item1.ContentType != contentType && value.Item1.ID == GirlID)), (Action<(ICard, int)>)delegate
		{
			AddProgress();
		});
	}

	private void SetProgress()
	{
		int progress = cardsCollectionTracker.TryGetGirlLevel(GirlID);
		Data.Progress = progress;
		onUpdate.OnNext((IObjective)this);
	}
}
