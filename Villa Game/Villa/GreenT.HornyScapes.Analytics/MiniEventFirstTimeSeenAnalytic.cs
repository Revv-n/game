using System;
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
		_calendarQueue.OnCalendarActiveNotNull(EventStructureType.Mini).SelectMany((Func<CalendarModel, IObservable<CalendarModel>>)OnMiniEventFirstTimeSeen).Subscribe(SendEventByPass)
			.AddTo(onNewStream);
	}

	private IObservable<CalendarModel> OnMiniEventFirstTimeSeen(CalendarModel calendarModel)
	{
		return from x in _minieventSettingsProvider.GetEvent(calendarModel.BalanceId).WasFirstTimeSeen.Skip(1)
			where x
			select x into _
			select calendarModel;
	}

	private MiniEvent GetEventFromCalendar(CalendarModel calendarModel)
	{
		return _minieventSettingsProvider.GetEvent(calendarModel.BalanceId);
	}

	public override void SendEventByPass(CalendarModel tuple)
	{
		int eventId = GetEventFromCalendar(tuple).EventId;
		int uniqID = tuple.UniqID;
		amplitude.AddEvent(new MiniEventFirstTimeSeenAmplitudeEvent(uniqID, eventId));
	}
}
