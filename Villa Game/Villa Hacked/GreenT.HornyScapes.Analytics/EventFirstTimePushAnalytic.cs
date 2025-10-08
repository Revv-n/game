using System;
using System.Collections.Generic;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CalendarModel>(Observable.SelectMany<CalendarModel, CalendarModel>(_calendarQueue.OnCalendarActiveNotNull(EventStructureType.Event), (Func<CalendarModel, IObservable<CalendarModel>>)OnEventFirstTimePushed), (Action<CalendarModel>)SendEventByPass), (ICollection<IDisposable>)onNewStream);
	}

	private IObservable<CalendarModel> OnEventFirstTimePushed(CalendarModel calendarModel)
	{
		return Observable.Select<bool, CalendarModel>(Observable.Where<bool>(Observable.Skip<bool>((IObservable<bool>)_eventSettingsProvider.GetEvent(calendarModel.BalanceId).WasFirstTimePushed, 1), (Func<bool, bool>)((bool x) => x)), (Func<bool, CalendarModel>)((bool _) => calendarModel));
	}

	private Event GetEventFromCalendar(CalendarModel calendarModel)
	{
		return _eventSettingsProvider.GetEvent(calendarModel.BalanceId);
	}

	public override void SendEventByPass(CalendarModel tuple)
	{
		int eventId = GetEventFromCalendar(tuple).EventId;
		int uniqID = tuple.UniqID;
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)new EventFirstTimePushAmplitudeEvent(uniqID, eventId));
	}
}
