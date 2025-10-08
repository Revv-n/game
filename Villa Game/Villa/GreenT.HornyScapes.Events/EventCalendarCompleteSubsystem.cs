using System;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.DisplayStrategy;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.IndicatorAdapter;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.MergeCore;
using GreenT.Types;
using Merge.Meta.RoomObjects;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventCalendarCompleteSubsystem : IInitializable, IDisposable
{
	private readonly GameStarter _gameStarter;

	private readonly CalendarQueue _calendarQueue;

	private readonly EventWindowOpener _eventWindowOpener;

	private readonly ContentSelectorGroup _contentSelectorGroup;

	private readonly SignalBus _signalBus;

	private readonly IndicatorDisplayService _indicatorDisplayService;

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	public EventCalendarCompleteSubsystem(CalendarQueue calendarQueue, EventWindowOpener eventWindowOpener, GameStarter gameStarter, ContentSelectorGroup contentSelectorGroup, SignalBus signalBus, IndicatorDisplayService indicatorDisplayService)
	{
		_signalBus = signalBus;
		_gameStarter = gameStarter;
		_calendarQueue = calendarQueue;
		_eventWindowOpener = eventWindowOpener;
		_contentSelectorGroup = contentSelectorGroup;
		_indicatorDisplayService = indicatorDisplayService;
	}

	public void Initialize()
	{
		_disposables?.Clear();
		_gameStarter.IsGameActive.Where((bool x) => x).SelectMany((bool _) => OnEventCalendarChangeState()).Where(IsEnd)
			.Do(OnEndEvent)
			.Where(IsComplete)
			.Do(delegate
			{
				_signalBus.TryFire(new IndicatorSignals.PushRequest(status: true, FilteredIndicatorType.EventProgress));
			})
			.SelectMany((CalendarModel calendar) => from _ in _indicatorDisplayService.OnIndicatorPush(FilteredIndicatorType.EventProgress).Do(delegate
				{
					_signalBus.TryFire(new IndicatorSignals.PushRequest(status: false, FilteredIndicatorType.EventProgress));
				}).Take(1)
				where _calendarQueue.IsCalendarActive(calendar)
				select _)
			.Subscribe(delegate
			{
				_eventWindowOpener.PrepareViewToEndEvent(EntityStatus.Complete);
				_eventWindowOpener.OpenProgress();
			})
			.AddTo(_disposables);
		static bool IsComplete(CalendarModel calendar)
		{
			return calendar.CalendarState.Value == EntityStatus.Complete;
		}
		static bool IsEnd(CalendarModel calendar)
		{
			EntityStatus value = calendar.CalendarState.Value;
			return value == EntityStatus.Complete || value == EntityStatus.Rewarded;
		}
		IObservable<CalendarModel> OnEventCalendarChangeState()
		{
			return _calendarQueue.OnCalendarActiveNotNull(EventStructureType.Event).SelectMany((CalendarModel calendar) => from _ in calendar.CalendarState.TakeWhile((EntityStatus _) => _calendarQueue.IsCalendarActive(calendar))
				select calendar).TakeUntil(from x in _gameStarter.IsGameActive.Skip(1)
				where !x
				select x);
		}
	}

	private void OnEndEvent(CalendarModel calendarModel)
	{
		try
		{
			_eventWindowOpener.PrepareViewToEndEvent(calendarModel.CalendarState.Value);
			_contentSelectorGroup.Select(ContentType.Main);
			Controller<GameItemController>.Instance.OpenField(ContentType.Main);
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
	}

	public void Dispose()
	{
		_disposables.Dispose();
	}
}
