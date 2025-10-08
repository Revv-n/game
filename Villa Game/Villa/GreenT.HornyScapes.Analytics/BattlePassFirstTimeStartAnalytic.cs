using System;
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
		(from x in _battlePassProvider.CalendarChangeProperty.Where(((CalendarModel calendar, BattlePass battlePass) x) => x.calendar != null).SelectMany<(CalendarModel, BattlePass), (CalendarModel, BattlePass)>((Func<(CalendarModel, BattlePass), IObservable<(CalendarModel, BattlePass)>>)SelectStarted).Skip(1)
			where x.battlePass.Data.StartData.WasFirstTimeStarted.Value
			select x).Subscribe(SendEventByPass).AddTo(onNewStream);
	}

	public override void SendEventByPass((CalendarModel calendarModel, BattlePass battlePass) tuple)
	{
		int uniqID = tuple.calendarModel.UniqID;
		int iD = tuple.battlePass.ID;
		amplitude.AddEvent(new BattlePassFirstStartAmplitudeEvent(uniqID, iD));
	}

	private IObservable<(CalendarModel calendar, BattlePass battlePass)> SelectStarted((CalendarModel calendarModel, BattlePass battlePass) tuple)
	{
		return tuple.battlePass.Data.StartData.WasFirstTimeStarted.Select((bool _) => tuple);
	}
}
