using System;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using UniRx;

namespace GreenT.HornyScapes.Analytics;

public class EventFirstTimeStartAnalytic : BaseEntityAnalytic<CalendarModel>
{
	private readonly EventSettingsProvider _eventSettingsProvider;

	private readonly CalendarQueue _calendarQueue;

	protected EventFirstTimeStartAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, CalendarQueue calendarQueue, EventSettingsProvider eventSettingsProvider)
		: base(amplitude)
	{
		_eventSettingsProvider = eventSettingsProvider;
		_calendarQueue = calendarQueue;
	}

	public override void Track()
	{
		ClearStreams();
		TrackFirstTimeStart();
	}

	private void TrackFirstTimeStart()
	{
		_calendarQueue.OnCalendarActiveNotNull(EventStructureType.Event).SelectMany((Func<CalendarModel, IObservable<CalendarModel>>)OnEventFirstTimeStarted).Subscribe(SendEventByPass)
			.AddTo(onNewStream);
	}

	private IObservable<CalendarModel> OnEventFirstTimeStarted(CalendarModel calendarModel)
	{
		return from x in _eventSettingsProvider.GetEvent(calendarModel.BalanceId).WasFirstTimeStarted.Skip(1)
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
		amplitude.AddEvent(new EventFirstTimeStartAmplitudeEvent(uniqID, eventId));
	}
}
