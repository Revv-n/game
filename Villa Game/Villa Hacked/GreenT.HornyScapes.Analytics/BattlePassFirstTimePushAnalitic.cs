using System;
using System.Collections.Generic;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(CalendarModel, BattlePass)>(Observable.Where<(CalendarModel, BattlePass)>(Observable.Skip<(CalendarModel, BattlePass)>(Observable.SelectMany<(CalendarModel, BattlePass), (CalendarModel, BattlePass)>(Observable.Where<(CalendarModel, BattlePass)>((IObservable<(CalendarModel, BattlePass)>)_battlePassProvider.CalendarChangeProperty, (Func<(CalendarModel, BattlePass), bool>)(((CalendarModel calendar, BattlePass battlePass) x) => x.calendar != null)), (Func<(CalendarModel, BattlePass), IObservable<(CalendarModel, BattlePass)>>)SubscribeAnalyticPush), 1), (Func<(CalendarModel, BattlePass), bool>)(((CalendarModel calendarModel, BattlePass battlePass) tuple) => tuple.battlePass.Data.StartData.WasFirstTimePushed.Value)), (Action<(CalendarModel, BattlePass)>)SendEventByPass), (ICollection<IDisposable>)onNewStream);
	}

	private IObservable<(CalendarModel calendarModel, BattlePass battlePass)> SubscribeAnalyticPush((CalendarModel calendarModel, BattlePass battlePass) tuple)
	{
		return Observable.Select<bool, (CalendarModel, BattlePass)>((IObservable<bool>)tuple.battlePass.Data.StartData.WasFirstTimePushed, (Func<bool, (CalendarModel, BattlePass)>)((bool _) => tuple));
	}

	public override void SendEventByPass((CalendarModel calendar, BattlePass battlePass) tuple)
	{
		int uniqID = tuple.calendar.UniqID;
		int iD = tuple.battlePass.ID;
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)new BattlePassFirstPushAmplitudeEvent(uniqID, iD));
	}
}
