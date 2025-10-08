using System;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Tasks;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes;

[Objective]
public sealed class EventAddCurrencyObjective : ConcreteCurrencyAddObjective
{
	private readonly CalendarQueue _calendarQueue;

	public readonly int TargetEventID;

	public EventAddCurrencyObjective(CalendarQueue calendarQueue, int target_event_id, Func<Sprite> iconProvider, CurrencyType currencyType, SavableObjectiveData data, ICurrencyProcessor currencyProcessor, int[] identificators)
		: base(iconProvider, currencyType, data, currencyProcessor, identificators)
	{
		_calendarQueue = calendarQueue;
		TargetEventID = target_event_id;
	}

	protected override void AddProgress(int value)
	{
		CalendarModel activeCalendar = _calendarQueue.GetActiveCalendar(EventStructureType.Event);
		if (activeCalendar != null && activeCalendar.BalanceId == TargetEventID)
		{
			base.AddProgress(value);
		}
	}
}
