using System;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public sealed class EventXPAddCurrencyObjective : ConcreteCurrencyAddObjective
{
	private readonly CalendarQueue _calendarQueue;

	public readonly int TargetEventID;

	public EventXPAddCurrencyObjective(CalendarQueue calendarQueue, int target_event_id, Func<Sprite> iconProvider, CurrencyType currencyType, SavableObjectiveData data, ICurrencyProcessor currencyProcessor, int[] identificators)
		: base(iconProvider, currencyType, data, currencyProcessor, identificators)
	{
		_calendarQueue = calendarQueue;
		TargetEventID = target_event_id;
	}

	public override void Track()
	{
		base.Track();
		SetProgress();
	}

	protected override void AddProgress(int value)
	{
		CalendarModel activeCalendar = _calendarQueue.GetActiveCalendar(EventStructureType.Event);
		if (activeCalendar != null && activeCalendar.BalanceId == TargetEventID)
		{
			base.AddProgress(value);
		}
	}

	private void SetProgress()
	{
		CalendarModel activeCalendar = _calendarQueue.GetActiveCalendar(EventStructureType.Event);
		if (activeCalendar != null && activeCalendar.BalanceId == TargetEventID)
		{
			Data.Progress = _currencyProcessor.GetCount(_currencyType, _compositeIdentificator);
		}
	}
}
