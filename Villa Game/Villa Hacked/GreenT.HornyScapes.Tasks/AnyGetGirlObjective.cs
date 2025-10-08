using System;
using GreenT.HornyScapes.Card;
using GreenT.Types;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class AnyGetGirlObjective : GainObjective
{
	protected readonly CardsCollectionTracker _cardsCollectionTracker;

	protected IDisposable _anyGetStream;

	public AnyGetGirlObjective(Func<Sprite> iconProvider, SavableObjectiveData data, CardsCollectionTracker cardsCollectionTracker)
		: base(iconProvider, data)
	{
		_cardsCollectionTracker = cardsCollectionTracker;
	}

	public override void Track()
	{
		base.Track();
		_anyGetStream?.Dispose();
		_anyGetStream = ObservableExtensions.Subscribe<(ICard, int)>(Observable.Where<(ICard, int)>(_cardsCollectionTracker.OnNewCardStream(), (Func<(ICard, int), bool>)(((ICard, int) value) => value.Item1.ContentType != ContentType.Event)), (Action<(ICard, int)>)delegate((ICard, int) value)
		{
			AddProgress(value.Item2);
		});
	}

	protected void AddProgress(int value)
	{
		Data.Progress += value;
		onUpdate.OnNext((IObjective)this);
	}

	public override void OnRewardTask()
	{
		base.OnRewardTask();
		_anyGetStream?.Dispose();
	}
}
