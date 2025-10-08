using System;
using System.Collections.Generic;
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_signalBus = signalBus;
		_gameStarter = gameStarter;
		_calendarQueue = calendarQueue;
		_eventWindowOpener = eventWindowOpener;
		_contentSelectorGroup = contentSelectorGroup;
		_indicatorDisplayService = indicatorDisplayService;
	}

	public void Initialize()
	{
		CompositeDisposable disposables = _disposables;
		if (disposables != null)
		{
			disposables.Clear();
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.SelectMany<CalendarModel, bool>(Observable.Do<CalendarModel>(Observable.Where<CalendarModel>(Observable.Do<CalendarModel>(Observable.Where<CalendarModel>(Observable.SelectMany<bool, CalendarModel>(Observable.Where<bool>((IObservable<bool>)_gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), (Func<bool, IObservable<CalendarModel>>)((bool _) => OnEventCalendarChangeState())), (Func<CalendarModel, bool>)IsEnd), (Action<CalendarModel>)OnEndEvent), (Func<CalendarModel, bool>)IsComplete), (Action<CalendarModel>)delegate
		{
			_signalBus.TryFire<IndicatorSignals.PushRequest>(new IndicatorSignals.PushRequest(status: true, FilteredIndicatorType.EventProgress));
		}), (Func<CalendarModel, IObservable<bool>>)((CalendarModel calendar) => Observable.Where<bool>(Observable.Take<bool>(Observable.Do<bool>(_indicatorDisplayService.OnIndicatorPush(FilteredIndicatorType.EventProgress), (Action<bool>)delegate
		{
			_signalBus.TryFire<IndicatorSignals.PushRequest>(new IndicatorSignals.PushRequest(status: false, FilteredIndicatorType.EventProgress));
		}), 1), (Func<bool, bool>)((bool _) => _calendarQueue.IsCalendarActive(calendar))))), (Action<bool>)delegate
		{
			_eventWindowOpener.PrepareViewToEndEvent(EntityStatus.Complete);
			_eventWindowOpener.OpenProgress();
		}), (ICollection<IDisposable>)_disposables);
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
			return Observable.TakeUntil<CalendarModel, bool>(Observable.SelectMany<CalendarModel, CalendarModel>(_calendarQueue.OnCalendarActiveNotNull(EventStructureType.Event), (Func<CalendarModel, IObservable<CalendarModel>>)((CalendarModel calendar) => Observable.Select<EntityStatus, CalendarModel>(Observable.TakeWhile<EntityStatus>((IObservable<EntityStatus>)calendar.CalendarState, (Func<EntityStatus, bool>)((EntityStatus _) => _calendarQueue.IsCalendarActive(calendar))), (Func<EntityStatus, CalendarModel>)((EntityStatus _) => calendar)))), Observable.Where<bool>(Observable.Skip<bool>((IObservable<bool>)_gameStarter.IsGameActive, 1), (Func<bool, bool>)((bool x) => !x)));
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
