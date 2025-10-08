using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.Model.Collections;
using StripClub.Extensions;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class CalendarManager : SimpleManager<CalendarModel>
{
	private readonly IClock _clock;

	[Inject]
	public CalendarManager(IClock clock)
	{
		_clock = clock;
	}

	public IEnumerable<T> Get<T>() where T : CalendarModel
	{
		return collection.OfType<T>();
	}

	public bool IsAnyCalendarLoading()
	{
		return collection.Any((CalendarModel calendar) => calendar.IsLoading);
	}

	public bool TryGetNextCalendar(List<CalendarModel> currentCalendars, out CalendarModel calendarModel)
	{
		if (!TryGetNovice(currentCalendars, out calendarModel))
		{
			return TryGetPeriodic(currentCalendars, out calendarModel);
		}
		return true;
	}

	public bool IsAvailableWithSameType(IEnumerable<CalendarModel> calendars, CalendarModel requester)
	{
		if (requester.EventType == EventStructureType.Mini)
		{
			return calendars.All((CalendarModel model) => model.UniqID != requester.UniqID);
		}
		return calendars.All((CalendarModel model) => model.EventType != requester.EventType);
	}

	public bool IsAvailableDueNovice(List<CalendarModel> calendars, CalendarModel requester)
	{
		if (requester.EventType == EventStructureType.Mini && requester is NoviceCalendar && calendars.All((CalendarModel calendar) => !(calendar is NoviceCalendar)))
		{
			return false;
		}
		if (calendars.All((CalendarModel calendar) => !(calendar is NoviceCalendar)))
		{
			return true;
		}
		if (requester.EventType == EventStructureType.Mini && calendars.Any((CalendarModel item) => item.EventType == EventStructureType.Mini))
		{
			return false;
		}
		return true;
	}

	public bool HasAnyActiveLastChance()
	{
		long dateNow = _clock.GetTime().ConvertToUnixTimestamp();
		return collection.Any((CalendarModel calendar) => dateNow < calendar.LastChanceTimeStamp && calendar.StartedTimeStamp < dateNow && calendar.CalendarLoadingState.Value == CalendarModelLoadingState.InProgress);
	}

	public override void Dispose()
	{
		base.Dispose();
		foreach (CalendarModel item in collection)
		{
			item?.Dispose();
		}
	}

	private bool TryGetPeriodic(List<CalendarModel> currentCalendars, out CalendarModel calendarModel)
	{
		calendarModel = (currentCalendars.Any() ? GetOtherOrDefault<PeriodicCalendar>(currentCalendars) : GetAvailableOrDefault<PeriodicCalendar>());
		if (calendarModel == null)
		{
			return false;
		}
		return true;
	}

	private DateTime ToDate(long timeStamp)
	{
		return TimeExtension.ConvertFromUnixTimestamp(timeStamp);
	}

	private bool TryGetNovice(List<CalendarModel> currentCalendars, out CalendarModel calendarModel)
	{
		calendarModel = null;
		calendarModel = GetOtherOrDefault<NoviceCalendar>(currentCalendars);
		if (calendarModel == null)
		{
			return false;
		}
		return true;
	}

	private CalendarModel GetAvailableOrDefault<T>() where T : CalendarModel
	{
		return Get<T>().FirstOrDefault((T calendar) => calendar.IsUnlockedAndNotPassed.Value);
	}

	private CalendarModel GetOtherOrDefault<T>(List<CalendarModel> calendars) where T : CalendarModel
	{
		return Get<T>().FirstOrDefault((T calendar) => IsCalendarAvailable(calendars, calendar));
	}

	public bool IsCalendarAvailable<T>(List<CalendarModel> calendars, T calendar) where T : CalendarModel
	{
		if (!calendar.IsUnlockedAndNotPassed.Value)
		{
			return false;
		}
		if (!calendar.IsActiveTime)
		{
			return false;
		}
		if (!IsAvailableDueNovice(calendars, calendar))
		{
			return false;
		}
		if (!IsAvailableWithSameType(calendars, calendar))
		{
			return false;
		}
		if (calendars.Any((CalendarModel c) => calendar.UniqID == c.UniqID))
		{
			return false;
		}
		return !HasAnyActiveLastChance();
	}
}
