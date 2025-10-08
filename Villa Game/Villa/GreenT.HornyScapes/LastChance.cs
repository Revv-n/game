using System;

namespace GreenT.HornyScapes;

[Serializable]
public sealed class LastChance
{
	public int EventId { get; private set; }

	public int CalendarId { get; private set; }

	public long StartDate { get; private set; }

	public long EndDate { get; private set; }

	public LastChanceType LastChanceType { get; private set; }

	public LastChance(int eventId, int calendarId, long startDate, long endDate, LastChanceType lastChanceType)
	{
		EventId = eventId;
		CalendarId = calendarId;
		StartDate = startDate;
		EndDate = endDate;
		LastChanceType = lastChanceType;
	}

	public void UpdateDates(long newStartDate, long newEndDate)
	{
		StartDate = newStartDate;
		EndDate = newEndDate;
	}
}
