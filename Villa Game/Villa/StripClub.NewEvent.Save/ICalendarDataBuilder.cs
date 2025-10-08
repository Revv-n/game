using GreenT.HornyScapes.BattlePassSpace;

namespace StripClub.NewEvent.Save;

public interface ICalendarDataBuilder
{
	void CreateData(CalendarModel calendarModel, string saveKey);
}
