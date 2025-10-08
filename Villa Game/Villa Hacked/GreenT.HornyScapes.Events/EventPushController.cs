using System;
using System.Collections.Generic;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.DisplayStrategy;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.IndicatorAdapter;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.UI;
using Merge.Meta.RoomObjects;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventPushController : IInitializable, IDisposable
{
	private readonly CalendarQueue _calendarQueue;

	private readonly WindowOpener progressWindowOpener;

	private readonly GameStarter gameStarter;

	private readonly EventProgressView eventProgressView;

	private readonly IndicatorDisplayService _indicatorDisplayService;

	private readonly EventSettingsProvider _eventSettingsProvider;

	private readonly SignalBus _signalBus;

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	public EventPushController(CalendarQueue calendarQueue, WindowOpener progressWindowOpener, GameStarter gameStarter, EventProgressView eventProgressView, EventSettingsProvider eventSettingsProvider, IndicatorDisplayService indicatorDisplayService, SignalBus signalBus)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_indicatorDisplayService = indicatorDisplayService;
		_eventSettingsProvider = eventSettingsProvider;
		_calendarQueue = calendarQueue;
		_signalBus = signalBus;
		this.progressWindowOpener = progressWindowOpener;
		this.gameStarter = gameStarter;
		this.eventProgressView = eventProgressView;
	}

	public void Initialize()
	{
		_compositeDisposable.Clear();
		IObservable<EntityStatus> observable = Observable.Select<CalendarModel, EntityStatus>(_calendarQueue.OnCalendarStateChange(EventStructureType.Event), (Func<CalendarModel, EntityStatus>)((CalendarModel p) => p.CalendarState.Value));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<EntityStatus>(Observable.Where<EntityStatus>(observable, (Func<EntityStatus, bool>)((EntityStatus calendarState) => calendarState == EntityStatus.InProgress)), (Action<EntityStatus>)delegate
		{
			_signalBus.TryFire<IndicatorSignals.PushRequest>(new IndicatorSignals.PushRequest(status: true, FilteredIndicatorType.EventStart));
		}), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.SelectMany<EntityStatus, bool>(Observable.DelayFrame<EntityStatus>(observable, 1, (FrameCountType)0), (Func<EntityStatus, IObservable<bool>>)((EntityStatus _) => PushOnceOnMeta())), (Action<bool>)delegate
		{
			eventProgressView.SetViewState(0);
			progressWindowOpener.Click();
			_signalBus.TryFire<IndicatorSignals.PushRequest>(new IndicatorSignals.PushRequest(status: false, FilteredIndicatorType.EventStart));
			CalendarModel activeCalendar = _calendarQueue.GetActiveCalendar(EventStructureType.Event);
			if (_eventSettingsProvider.TryGetEvent(activeCalendar.BalanceId, out var @event))
			{
				@event.SetFirstTimePushed();
			}
		}), (ICollection<IDisposable>)_compositeDisposable);
	}

	private IObservable<bool> PushOnceOnMeta()
	{
		return Observable.FirstOrDefault<bool>(Observable.CombineLatest<bool, bool, bool>(_indicatorDisplayService.OnIndicatorPush(FilteredIndicatorType.EventStart), (IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool, bool>)((bool isVisible, bool gameIsActive) => isVisible && gameIsActive)), (Func<bool, bool>)((bool x) => x));
	}

	public void Dispose()
	{
		_compositeDisposable.Dispose();
	}
}
