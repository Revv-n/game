using System;
using GreenT.HornyScapes.Events;
using StripClub.NewEvent.Data;
using UniRx;

namespace GreenT.HornyScapes.BattlePassSpace;

public class BattlePassNotifyService : ICalendarQueueListener, IDisposable
{
	private readonly BattlePassProvider _battlePassProvider;

	private readonly BattlePassSettingsProvider _battlePassSettingsProvider;

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	public BattlePassNotifyService(BattlePassProvider battlePassProvider, BattlePassSettingsProvider battlePassSettingsProvider)
	{
		_battlePassProvider = battlePassProvider;
		_battlePassSettingsProvider = battlePassSettingsProvider;
	}

	public void Initialize(CalendarQueue calendarQueue)
	{
		_compositeDisposable.Clear();
		IObservable<(CalendarModel calendar, BattlePass)> source = from calendar in calendarQueue.OnCalendarActiveNotNull(EventStructureType.BattlePass)
			select (calendar: calendar, _battlePassSettingsProvider.GetBattlePass(calendar.BalanceId));
		source.Where<(CalendarModel, BattlePass)>(((CalendarModel calendar, BattlePass battlePass) tuple) => tuple.battlePass == null).Subscribe(delegate
		{
			_battlePassProvider.Reset();
		}).AddTo(_compositeDisposable);
		source.Where<(CalendarModel, BattlePass)>(((CalendarModel calendar, BattlePass battlePass) tuple) => tuple.battlePass != null).Subscribe(_battlePassProvider.Set).AddTo(_compositeDisposable);
		(from calendar in calendarQueue.OnCalendarEnd(EventStructureType.BattlePass)
			where calendar != null
			select _battlePassSettingsProvider.GetBattlePass(calendar.BalanceId) into battlePass
			where battlePass != null
			select battlePass).Subscribe(delegate
		{
			_battlePassProvider.Reset();
		}).AddTo(_compositeDisposable);
	}

	public void Dispose()
	{
		_compositeDisposable.Dispose();
	}
}
