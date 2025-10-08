using System;
using GreenT.HornyScapes.BattlePassSpace;

namespace GreenT.HornyScapes.Events;

public interface ICalendarLoader
{
	void SetLoadingStream(CalendarModel calendarModel);

	IObservable<CalendarModel> Load(CalendarModel calendarModel);

	IEventMapper GetEventMapper(int event_id);

	IObservable<CalendarModel> OnConcreteCalendarLoadingStateChange(EventStructureType type, CalendarLoadingStatus loadingStatus);
}
