using System;
using GreenT.HornyScapes.Card;
using GreenT.Types;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class AnyGirlPromoteObjective : GainObjective
{
	protected readonly CardsCollectionTracker cardsCollectionTracker;

	protected readonly ContentType contentType;

	protected IDisposable anyPromoteStream;

	public AnyGirlPromoteObjective(Func<Sprite> iconProvider, SavableObjectiveData data, CardsCollectionTracker cardsCollectionTracker, ContentType contentType)
		: base(iconProvider, data)
	{
		this.cardsCollectionTracker = cardsCollectionTracker;
		this.contentType = contentType;
	}

	public override void Track()
	{
		base.Track();
		anyPromoteStream?.Dispose();
		anyPromoteStream = ObservableExtensions.Subscribe<ICard>(Observable.Where<ICard>(cardsCollectionTracker.GetAnyPromoteStream(), (Func<ICard, bool>)((ICard _card) => _card.ContentType == contentType)), (Action<ICard>)delegate
		{
			AddProgress();
		});
	}

	protected void AddProgress()
	{
		Data.Progress++;
		onUpdate.OnNext((IObjective)this);
	}

	public override void OnRewardTask()
	{
		base.OnRewardTask();
		anyPromoteStream?.Dispose();
	}
}
