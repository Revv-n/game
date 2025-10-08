using System;
using System.Collections.Generic;
using GreenT.HornyScapes.BattlePassSpace;
using UniRx;

namespace GreenT.HornyScapes.Analytics;

public class BattlePassFirstTimeStartAnalytic : BaseEntityAnalytic<(CalendarModel calendarModel, BattlePass battlePass)>
{
	private readonly BattlePassProvider _battlePassProvider;

	public BattlePassFirstTimeStartAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, BattlePassProvider battlePassProvider)
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(CalendarModel, BattlePass)>(Observable.Where<(CalendarModel, BattlePass)>(Observable.Skip<(CalendarModel, BattlePass)>(Observable.SelectMany<(CalendarModel, BattlePass), (CalendarModel, BattlePass)>(Observable.Where<(CalendarModel, BattlePass)>((IObservable<(CalendarModel, BattlePass)>)_battlePassProvider.CalendarChangeProperty, (Func<(CalendarModel, BattlePass), bool>)(((CalendarModel calendar, BattlePass battlePass) x) => x.calendar != null)), (Func<(CalendarModel, BattlePass), IObservable<(CalendarModel, BattlePass)>>)SelectStarted), 1), (Func<(CalendarModel, BattlePass), bool>)(((CalendarModel calendar, BattlePass battlePass) x) => x.battlePass.Data.StartData.WasFirstTimeStarted.Value)), (Action<(CalendarModel, BattlePass)>)SendEventByPass), (ICollection<IDisposable>)onNewStream);
	}

	public override void SendEventByPass((CalendarModel calendarModel, BattlePass battlePass) tuple)
	{
		int uniqID = tuple.calendarModel.UniqID;
		int iD = tuple.battlePass.ID;
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)new BattlePassFirstStartAmplitudeEvent(uniqID, iD));
	}

	private IObservable<(CalendarModel calendar, BattlePass battlePass)> SelectStarted((CalendarModel calendarModel, BattlePass battlePass) tuple)
	{
		return Observable.Select<bool, (CalendarModel, BattlePass)>((IObservable<bool>)tuple.battlePass.Data.StartData.WasFirstTimeStarted, (Func<bool, (CalendarModel, BattlePass)>)((bool _) => tuple));
	}
}
