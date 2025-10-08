using System;
using GreenT.HornyScapes.BattlePassSpace;

namespace GreenT.HornyScapes._HornyScapes._Scripts.Events.Calendar;

public interface ICalendarStrategy
{
	void Build(CalendarModel calendarModel, string saveKey);

	IObservable<CalendarModel> Load(CalendarModel calendarModel);

	void Dispense(int balanceId);

	void Clean(CalendarModel calendarModel);
}
