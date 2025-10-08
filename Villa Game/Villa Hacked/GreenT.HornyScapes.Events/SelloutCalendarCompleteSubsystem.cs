using System;
using System.Collections.Generic;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.DisplayStrategy;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.IndicatorAdapter;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events.Content;
using Merge.Meta.RoomObjects;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class SelloutCalendarCompleteSubsystem : IInitializable, IDisposable
{
	private readonly CalendarQueue _calendarQueue;

	private readonly BattlePassRewardedWindowOpener battlePassRewardedWindowOpener;

	private readonly ContentSelectorGroup _contentSelectorGroup;

	private readonly IndicatorDisplayService _displayService;

	private readonly GameStarter _gameStarter;

	private readonly SignalBus _signalBus;

	private readonly IndicatorDisplayService _indicatorDisplayService;

	private readonly CompositeDisposable disposables = new CompositeDisposable();

	public SelloutCalendarCompleteSubsystem(CalendarQueue calendarQueue, BattlePassRewardedWindowOpener battlePassRewardedWindowOpener, ContentSelectorGroup contentSelectorGroup, GameStarter gameStarter, IndicatorDisplayService indicatorDisplayService, SignalBus signalBus)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_indicatorDisplayService = indicatorDisplayService;
		_gameStarter = gameStarter;
		_calendarQueue = calendarQueue;
		_signalBus = signalBus;
		this.battlePassRewardedWindowOpener = battlePassRewardedWindowOpener;
		_contentSelectorGroup = contentSelectorGroup;
	}

	public void Initialize()
	{
		disposables.Clear();
		IObservable<CalendarModel> observable = Observable.Where<CalendarModel>(Observable.SelectMany<bool, CalendarModel>(Observable.Where<bool>((IObservable<bool>)_gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), (Func<bool, IObservable<CalendarModel>>)((bool _) => OnCalendarChangeState())), (Func<CalendarModel, bool>)IsEnd);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CalendarModel>(Observable.Where<CalendarModel>(observable, (Func<CalendarModel, bool>)IsRewarded), (Action<CalendarModel>)delegate
		{
			battlePassRewardedWindowOpener.CloseProgress();
		}), (ICollection<IDisposable>)disposables);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CalendarModel>(Observable.SelectMany<CalendarModel, CalendarModel>(Observable.Do<CalendarModel>(Observable.Where<CalendarModel>(observable, (Func<CalendarModel, bool>)IsComplete), (Action<CalendarModel>)delegate
		{
			_signalBus.TryFire<IndicatorSignals.PushRequest>(new IndicatorSignals.PushRequest(status: true, FilteredIndicatorType.BattlePassProgress));
		}), (Func<CalendarModel, IObservable<CalendarModel>>)((CalendarModel calendar) => Observable.Select<bool, CalendarModel>(Observable.Where<bool>(Observable.Take<bool>(Observable.Do<bool>(_indicatorDisplayService.OnIndicatorPush(FilteredIndicatorType.BattlePassProgress), (Action<bool>)delegate
		{
			_signalBus.TryFire<IndicatorSignals.PushRequest>(new IndicatorSignals.PushRequest(status: false, FilteredIndicatorType.BattlePassProgress));
		}), 1), (Func<bool, bool>)((bool _) => _calendarQueue.IsCalendarActive(calendar))), (Func<bool, CalendarModel>)((bool _) => calendar)))), (Action<CalendarModel>)delegate
		{
			battlePassRewardedWindowOpener.OpenProgress();
		}), (ICollection<IDisposable>)disposables);
		static bool IsComplete(CalendarModel calendar)
		{
			return calendar.CalendarState.Value == EntityStatus.Complete;
		}
		static bool IsEnd(CalendarModel calendar)
		{
			if (!IsComplete(calendar))
			{
				return IsRewarded(calendar);
			}
			return true;
		}
		static bool IsRewarded(CalendarModel calendar)
		{
			return calendar.CalendarState.Value == EntityStatus.Rewarded;
		}
	}

	private IObservable<CalendarModel> OnCalendarChangeState()
	{
		return Observable.TakeUntil<CalendarModel, bool>(Observable.SelectMany<CalendarModel, CalendarModel>(_calendarQueue.OnCalendarActiveNotNull(EventStructureType.BattlePass), (Func<CalendarModel, IObservable<CalendarModel>>)((CalendarModel calendar) => Observable.Select<EntityStatus, CalendarModel>(Observable.TakeWhile<EntityStatus>((IObservable<EntityStatus>)calendar.CalendarState, (Func<EntityStatus, bool>)((EntityStatus _) => _calendarQueue.IsCalendarActive(calendar))), (Func<EntityStatus, CalendarModel>)((EntityStatus _) => calendar)))), Observable.Where<bool>(Observable.Skip<bool>((IObservable<bool>)_gameStarter.IsGameActive, 1), (Func<bool, bool>)((bool x) => !x)));
	}

	public void Dispose()
	{
		disposables.Dispose();
	}
}
