using GreenT.HornyScapes;

namespace StripClub.NewEvent.Data;

public interface ICalendarQueueListener
{
	void Initialize(CalendarQueue calendarQueue);
}
