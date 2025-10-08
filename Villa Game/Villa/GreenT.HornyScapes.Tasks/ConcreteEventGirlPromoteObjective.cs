using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Card;
using GreenT.HornyScapes.Events;
using GreenT.Types;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public sealed class ConcreteEventGirlPromoteObjective : AnyGirlPromoteObjective
{
	private readonly CardsCollection _cardsCollection;

	private readonly CalendarQueue _calendarQueue;

	public readonly int TargetEventID;

	public ConcreteEventGirlPromoteObjective(Func<Sprite> iconProvider, SavableObjectiveData data, CardsCollectionTracker cardsCollectionTracker, CardsCollection cardsCollection, int targetEventID, CalendarQueue calendarQueue)
		: base(iconProvider, data, cardsCollectionTracker, ContentType.Event)
	{
		_cardsCollection = cardsCollection;
		_calendarQueue = calendarQueue;
		TargetEventID = targetEventID;
	}

	public override void Track()
	{
		SetProgress();
		anyPromoteStream?.Dispose();
		anyPromoteStream = (from value in cardsCollectionTracker.GetAnyPromoteStream()
			where value.ContentType == contentType
			select value).Subscribe(delegate
		{
			AddProgress();
		});
	}

	private void SetProgress()
	{
		CalendarModel activeCalendar = _calendarQueue.GetActiveCalendar(EventStructureType.Event);
		if (activeCalendar != null && activeCalendar.BalanceId == TargetEventID)
		{
			IEnumerable<ICard> source = _cardsCollection.Collection.Where((ICard _card) => _card.ContentType == ContentType.Event && _cardsCollection.Promote.ContainsKey(_card));
			int num = source.Sum((ICard card) => _cardsCollection.Promote[card].Level.Value);
			num -= source.Count();
			Data.Progress = num;
			onUpdate.OnNext(this);
		}
	}
}
