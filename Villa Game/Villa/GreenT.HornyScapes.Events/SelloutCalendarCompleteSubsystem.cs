using System;
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
		IObservable<CalendarModel> source = _gameStarter.IsGameActive.Where((bool x) => x).SelectMany((bool _) => OnCalendarChangeState()).Where(IsEnd);
		source.Where(IsRewarded).Subscribe(delegate
		{
			battlePassRewardedWindowOpener.CloseProgress();
		}).AddTo(disposables);
		source.Where(IsComplete).Do(delegate
		{
			_signalBus.TryFire(new IndicatorSignals.PushRequest(status: true, FilteredIndicatorType.BattlePassProgress));
		}).SelectMany((CalendarModel calendar) => from _ in _indicatorDisplayService.OnIndicatorPush(FilteredIndicatorType.BattlePassProgress).Do(delegate
			{
				_signalBus.TryFire(new IndicatorSignals.PushRequest(status: false, FilteredIndicatorType.BattlePassProgress));
			}).Take(1)
			where _calendarQueue.IsCalendarActive(calendar)
			select calendar)
			.Subscribe(delegate
			{
				battlePassRewardedWindowOpener.OpenProgress();
			})
			.AddTo(disposables);
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
		return _calendarQueue.OnCalendarActiveNotNull(EventStructureType.BattlePass).SelectMany((CalendarModel calendar) => from _ in calendar.CalendarState.TakeWhile((EntityStatus _) => _calendarQueue.IsCalendarActive(calendar))
			select calendar).TakeUntil(from x in _gameStarter.IsGameActive.Skip(1)
			where !x
			select x);
	}

	public void Dispose()
	{
		disposables.Dispose();
	}
}
