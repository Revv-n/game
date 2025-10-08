using System;
using System.Collections.Generic;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using UniRx;

namespace GreenT.HornyScapes.Analytics;

public sealed class MiniEventFirstTimeSeenAnalytic : BaseEntityAnalytic<CalendarModel>
{
	private readonly MiniEventSettingsProvider _minieventSettingsProvider;

	private readonly CalendarQueue _calendarQueue;

	public MiniEventFirstTimeSeenAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, CalendarQueue calendarQueue, MiniEventSettingsProvider minieventSettingsProvider)
		: base(amplitude)
	{
		_minieventSettingsProvider = minieventSettingsProvider;
		_calendarQueue = calendarQueue;
	}

	public override void Track()
	{
		ClearStreams();
		TrackFirstTimeSeen();
	}

	private void TrackFirstTimeSeen()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CalendarModel>(Observable.SelectMany<CalendarModel, CalendarModel>(_calendarQueue.OnCalendarActiveNotNull(EventStructureType.Mini), (Func<CalendarModel, IObservable<CalendarModel>>)OnMiniEventFirstTimeSeen), (Action<CalendarModel>)SendEventByPass), (ICollection<IDisposable>)onNewStream);
	}

	private IObservable<CalendarModel> OnMiniEventFirstTimeSeen(CalendarModel calendarModel)
	{
		return Observable.Select<bool, CalendarModel>(Observable.Where<bool>(Observable.Skip<bool>((IObservable<bool>)_minieventSettingsProvider.GetEvent(calendarModel.BalanceId).WasFirstTimeSeen, 1), (Func<bool, bool>)((bool x) => x)), (Func<bool, CalendarModel>)((bool _) => calendarModel));
	}

	private MiniEvent GetEventFromCalendar(CalendarModel calendarModel)
	{
		return _minieventSettingsProvider.GetEvent(calendarModel.BalanceId);
	}

	public override void SendEventByPass(CalendarModel tuple)
	{
		int eventId = GetEventFromCalendar(tuple).EventId;
		int uniqID = tuple.UniqID;
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)new MiniEventFirstTimeSeenAmplitudeEvent(uniqID, eventId));
	}
}
