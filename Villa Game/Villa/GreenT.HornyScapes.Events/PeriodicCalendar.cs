using System;
using GreenT.Data;
using GreenT.HornyScapes._HornyScapes._Scripts.Events.Calendar;
using GreenT.HornyScapes.BattlePassSpace;
using Merge.Meta.RoomObjects;
using StripClub.Extensions;
using StripClub.Model;
using StripClub.UI;
using UniRx;

namespace GreenT.HornyScapes.Events;

[MementoHolder]
public class PeriodicCalendar : CalendarModel, IComparable<PeriodicCalendar>
{
	[Serializable]
	public class PeriodicCalendarMemento : CalendarMemento
	{
		public PeriodicCalendarMemento(PeriodicCalendar savableState)
			: base(savableState)
		{
		}
	}

	private string uniqueKey;

	private PeriodicCalendarMemento savedCalendar;

	public GenericTimer StartTimer { get; }

	public DateTime ComingSoonDate { get; }

	public DateTime EndDate { get; }

	public long EndTimeStamp => base.StartedTimeStamp + duration;

	public override bool IsNotPassedCalendar
	{
		get
		{
			bool flag = EndTimeStamp > clock.GetTime().ConvertToUnixTimestamp();
			if (WasStarted || flag)
			{
				return base.IsNotPassedCalendar;
			}
			return false;
		}
	}

	public PeriodicCalendar(int uniqId, EventStructureType structureType, IEventMapper eventMapper, int duration, ILocker[] locker, int showPromo, long startDate, IClock clock, ICalendarStrategy calendarStrategy, long lastChanceDuration)
		: base(uniqId, structureType, eventMapper, duration, locker, clock, calendarStrategy, lastChanceDuration)
	{
		int uniqID = UniqID;
		uniqueKey = "periodic_calendar_" + uniqID;
		base.StartedTimeStamp = startDate;
		StartTimer = new GenericTimer(TimeSpan.Zero);
		EndDate = TimeExtension.ConvertFromUnixTimestamp(base.StartedTimeStamp + duration);
		base.ComingSoonTimeStamp = startDate - showPromo;
		ComingSoonDate = TimeExtension.ConvertFromUnixTimestamp(base.ComingSoonTimeStamp);
		base.LastChanceTimeStamp = ((0 < lastChanceDuration) ? (startDate + duration + lastChanceDuration) : 0);
	}

	public override void Initialize()
	{
		base.Initialize();
		TimeSpan timeLeft = ComingSoonDate - clock.GetTime();
		if (timeLeft.Ticks <= 0)
		{
			return;
		}
		StartTimer.Start(timeLeft);
		base.IsUnlockedAndNotPassed = base.IsUnlockedAndNotPassed.SelectMany((bool available) => StartTimer.OnTimeIsUp.Select((GenericTimer _) => available)).ToReadOnlyReactiveProperty(initialValue: false);
	}

	public override void Launch()
	{
		_timerStream.Clear();
		long num = CalculateComingSoonDuration();
		ComingSoonTimer.Start(TimeSpan.FromSeconds(num));
		IObservable<long> source = from _ in ComingSoonTimer.OnTimeIsUp.Take(1)
			select base.RemainingTime;
		source.Where((long _leftTime) => _leftTime > 0).Subscribe(delegate
		{
			SetInProgress();
		}).AddTo(_timerStream);
		source.Where((long _leftTime) => _leftTime <= 0).Subscribe(delegate
		{
			SetComplete();
		}).AddTo(_timerStream);
		base.Launch();
	}

	private long CalculateComingSoonDuration()
	{
		long num = clock.GetTime().ConvertToUnixTimestamp();
		long result = 0L;
		if (num >= base.ComingSoonTimeStamp && num < base.StartedTimeStamp)
		{
			result = base.StartedTimeStamp - num;
		}
		return result;
	}

	public int CompareTo(PeriodicCalendar other)
	{
		return base.StartedTimeStamp.CompareTo(other.StartedTimeStamp);
	}

	public override string UniqueKey()
	{
		return uniqueKey;
	}

	public override Memento SaveState()
	{
		return new PeriodicCalendarMemento(this);
	}

	public override void LoadState(Memento memento)
	{
		PeriodicCalendarMemento periodicCalendarMemento = (PeriodicCalendarMemento)memento;
		WasStarted = periodicCalendarMemento.WasStarted;
		WasEnded = periodicCalendarMemento.WasEnded;
		EventEnergyResetToStart = periodicCalendarMemento.EventEnergyResetToStart;
		if (periodicCalendarMemento.EntityStatus == EntityStatus.Rewarded)
		{
			CalendarState.Value = EntityStatus.Rewarded;
		}
	}
}
