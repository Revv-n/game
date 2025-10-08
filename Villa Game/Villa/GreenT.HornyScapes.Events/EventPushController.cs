using System;
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
		IObservable<EntityStatus> source = from p in _calendarQueue.OnCalendarStateChange(EventStructureType.Event)
			select p.CalendarState.Value;
		source.Where((EntityStatus calendarState) => calendarState == EntityStatus.InProgress).Subscribe(delegate
		{
			_signalBus.TryFire(new IndicatorSignals.PushRequest(status: true, FilteredIndicatorType.EventStart));
		}).AddTo(_compositeDisposable);
		source.DelayFrame(1).SelectMany((EntityStatus _) => PushOnceOnMeta()).Subscribe(delegate
		{
			eventProgressView.SetViewState(0);
			progressWindowOpener.Click();
			_signalBus.TryFire(new IndicatorSignals.PushRequest(status: false, FilteredIndicatorType.EventStart));
			CalendarModel activeCalendar = _calendarQueue.GetActiveCalendar(EventStructureType.Event);
			if (_eventSettingsProvider.TryGetEvent(activeCalendar.BalanceId, out var @event))
			{
				@event.SetFirstTimePushed();
			}
		})
			.AddTo(_compositeDisposable);
	}

	private IObservable<bool> PushOnceOnMeta()
	{
		return _indicatorDisplayService.OnIndicatorPush(FilteredIndicatorType.EventStart).CombineLatest(gameStarter.IsGameActive, (bool isVisible, bool gameIsActive) => isVisible && gameIsActive).FirstOrDefault((bool x) => x);
	}

	public void Dispose()
	{
		_compositeDisposable.Dispose();
	}
}
