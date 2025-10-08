using System;
using System.Collections.Generic;
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_battlePassProvider = battlePassProvider;
		_battlePassSettingsProvider = battlePassSettingsProvider;
	}

	public void Initialize(CalendarQueue calendarQueue)
	{
		_compositeDisposable.Clear();
		IObservable<(CalendarModel, BattlePass)> observable = Observable.Select<CalendarModel, (CalendarModel, BattlePass)>(calendarQueue.OnCalendarActiveNotNull(EventStructureType.BattlePass), (Func<CalendarModel, (CalendarModel, BattlePass)>)((CalendarModel calendar) => (calendar: calendar, _battlePassSettingsProvider.GetBattlePass(calendar.BalanceId))));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(CalendarModel, BattlePass)>(Observable.Where<(CalendarModel, BattlePass)>(observable, (Func<(CalendarModel, BattlePass), bool>)(((CalendarModel calendar, BattlePass battlePass) tuple) => tuple.battlePass == null)), (Action<(CalendarModel, BattlePass)>)delegate
		{
			_battlePassProvider.Reset();
		}), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(CalendarModel, BattlePass)>(Observable.Where<(CalendarModel, BattlePass)>(observable, (Func<(CalendarModel, BattlePass), bool>)(((CalendarModel calendar, BattlePass battlePass) tuple) => tuple.battlePass != null)), (Action<(CalendarModel, BattlePass)>)_battlePassProvider.Set), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<BattlePass>(Observable.Where<BattlePass>(Observable.Select<CalendarModel, BattlePass>(Observable.Where<CalendarModel>(calendarQueue.OnCalendarEnd(EventStructureType.BattlePass), (Func<CalendarModel, bool>)((CalendarModel calendar) => calendar != null)), (Func<CalendarModel, BattlePass>)((CalendarModel calendar) => _battlePassSettingsProvider.GetBattlePass(calendar.BalanceId))), (Func<BattlePass, bool>)((BattlePass battlePass) => battlePass != null)), (Action<BattlePass>)delegate
		{
			_battlePassProvider.Reset();
		}), (ICollection<IDisposable>)_compositeDisposable);
	}

	public void Dispose()
	{
		_compositeDisposable.Dispose();
	}
}
