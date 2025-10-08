using System;
using GreenT.Data;
using GreenT.HornyScapes._HornyScapes._Scripts.Events.Calendar;
using GreenT.HornyScapes.BattlePassSpace;
using StripClub.Extensions;
using StripClub.Model;

namespace GreenT.HornyScapes.Events;

[MementoHolder]
public class NoviceCalendar : CalendarModel
{
	[Serializable]
	public class NoviceCalendarMemento : CalendarMemento
	{
		public long StartedTimeStamp;

		public long LastChanceTimeStamp;

		public NoviceCalendarMemento(NoviceCalendar savableState)
			: base(savableState)
		{
			StartedTimeStamp = savableState.StartedTimeStamp;
			LastChanceTimeStamp = savableState.LastChanceTimeStamp;
		}
	}

	private readonly long _lastChanceDuration;

	private readonly string _uniqueKey;

	private NoviceCalendarMemento _savedCalendar;

	private bool WasLaunched => base.StartedTimeStamp != 0;

	public NoviceCalendar(EventStructureType structureType, IEventMapper eventMapper, int duration, ILocker[] locker, int uniqID, IClock clock, ICalendarStrategy calendarStrategy, long lastChanceDuration)
		: base(uniqID, structureType, eventMapper, duration, locker, clock, calendarStrategy, lastChanceDuration)
	{
		int uniqID2 = UniqID;
		_uniqueKey = "novice_calendar_" + uniqID2;
		_lastChanceDuration = lastChanceDuration;
	}

	public override void Launch()
	{
		base.Launch();
		if (!WasLaunched)
		{
			base.StartedTimeStamp = clock.GetTime().ConvertToUnixTimestamp();
			base.LastChanceTimeStamp = ((0 < _lastChanceDuration) ? (base.StartedTimeStamp + duration + _lastChanceDuration) : 0);
		}
		SetInProgress();
	}

	public override string UniqueKey()
	{
		return _uniqueKey;
	}

	public override Memento SaveState()
	{
		return new NoviceCalendarMemento(this);
	}

	public override void LoadState(Memento memento)
	{
		_savedCalendar = (NoviceCalendarMemento)memento;
		WasStarted = _savedCalendar.WasStarted;
		WasEnded = _savedCalendar.WasEnded;
		base.StartedTimeStamp = _savedCalendar.StartedTimeStamp;
		base.LastChanceTimeStamp = _savedCalendar.LastChanceTimeStamp;
		CalendarState.Value = _savedCalendar.EntityStatus;
	}
}
