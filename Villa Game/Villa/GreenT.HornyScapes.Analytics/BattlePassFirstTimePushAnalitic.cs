using System;
using GreenT.HornyScapes.BattlePassSpace;
using UniRx;

namespace GreenT.HornyScapes.Analytics;

public class BattlePassFirstTimePushAnalitic : BaseEntityAnalytic<(CalendarModel calendar, BattlePass battlePass)>
{
	private readonly BattlePassProvider _battlePassProvider;

	public BattlePassFirstTimePushAnalitic(IAmplitudeSender<AmplitudeEvent> amplitude, BattlePassProvider battlePassProvider)
		: base(amplitude)
	{
		_battlePassProvider = battlePassProvider;
	}

	public override void Track()
	{
		ClearStreams();
		TrackFirstTimeStart();
	}

	private void TrackFirstTimeStart()
	{
		(from tuple in _battlePassProvider.CalendarChangeProperty.Where(((CalendarModel calendar, BattlePass battlePass) x) => x.calendar != null).SelectMany<(CalendarModel, BattlePass), (CalendarModel, BattlePass)>((Func<(CalendarModel, BattlePass), IObservable<(CalendarModel, BattlePass)>>)SubscribeAnalyticPush).Skip(1)
			where tuple.battlePass.Data.StartData.WasFirstTimePushed.Value
			select tuple).Subscribe(SendEventByPass).AddTo(onNewStream);
	}

	private IObservable<(CalendarModel calendarModel, BattlePass battlePass)> SubscribeAnalyticPush((CalendarModel calendarModel, BattlePass battlePass) tuple)
	{
		return tuple.battlePass.Data.StartData.WasFirstTimePushed.Select((bool _) => tuple);
	}

	public override void SendEventByPass((CalendarModel calendar, BattlePass battlePass) tuple)
	{
		int uniqID = tuple.calendar.UniqID;
		int iD = tuple.battlePass.ID;
		amplitude.AddEvent(new BattlePassFirstPushAmplitudeEvent(uniqID, iD));
	}
}
