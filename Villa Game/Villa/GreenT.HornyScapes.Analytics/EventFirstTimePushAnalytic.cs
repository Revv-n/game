using System;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using UniRx;

namespace GreenT.HornyScapes.Analytics;

public class EventFirstTimePushAnalytic : BaseEntityAnalytic<CalendarModel>
{
	private readonly EventSettingsProvider _eventSettingsProvider;

	private readonly CalendarQueue _calendarQueue;

	protected EventFirstTimePushAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, CalendarQueue calendarQueue, EventSettingsProvider eventSettingsProvider)
		: base(amplitude)
	{
		_eventSettingsProvider = eventSettingsProvider;
		_calendarQueue = calendarQueue;
	}

	public override void Track()
	{
		ClearStreams();
		TrackFirstTimePush();
	}

	private void TrackFirstTimePush()
	{
		_calendarQueue.OnCalendarActiveNotNull(EventStructureType.Event).SelectMany((Func<CalendarModel, IObservable<CalendarModel>>)OnEventFirstTimePushed).Subscribe(SendEventByPass)
			.AddTo(onNewStream);
	}

	private IObservable<CalendarModel> OnEventFirstTimePushed(CalendarModel calendarModel)
	{
		return from x in _eventSettingsProvider.GetEvent(calendarModel.BalanceId).WasFirstTimePushed.Skip(1)
			where x
			select x into _
			select calendarModel;
	}

	private Event GetEventFromCalendar(CalendarModel calendarModel)
	{
		return _eventSettingsProvider.GetEvent(calendarModel.BalanceId);
	}

	public override void SendEventByPass(CalendarModel tuple)
	{
		int eventId = GetEventFromCalendar(tuple).EventId;
		int uniqID = tuple.UniqID;
		amplitude.AddEvent(new EventFirstTimePushAmplitudeEvent(uniqID, eventId));
	}
}
