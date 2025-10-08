using System;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.DisplayStrategy;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.IndicatorAdapter;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using GreenT.UI;
using JetBrains.Annotations;
using Merge.Meta.RoomObjects;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
public class MiniEventsPushController : IInitializable, IDisposable
{
	private readonly CalendarQueue _calendarQueue;

	private readonly WindowID _startWindowID;

	private readonly IWindowsManager _windowsManager;

	private readonly IndicatorDisplayService _indicatorDisplayService;

	private readonly SignalBus _signalBus;

	private readonly MiniEventSettingsProvider _miniEventSettingsProvider;

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	private MiniEventsPromoPusherView _miniEventsPromoPusherView;

	public MiniEventsPushController(CalendarQueue calendarQueue, WindowID startWindowID, MiniEventSettingsProvider miniEventSettingsProvider, IndicatorDisplayService indicatorDisplayService, SignalBus signalBus, IWindowsManager windowsManager)
	{
		_signalBus = signalBus;
		_windowsManager = windowsManager;
		_calendarQueue = calendarQueue;
		_startWindowID = startWindowID;
		_indicatorDisplayService = indicatorDisplayService;
		_miniEventSettingsProvider = miniEventSettingsProvider;
	}

	public void Initialize()
	{
		_compositeDisposable.Clear();
		(from calendar in (from calendar in _calendarQueue.OnCalendarStateChange(EventStructureType.Mini)
				where calendar.CalendarState.Value == EntityStatus.InProgress
				select calendar).DelayFrame(1).Do(delegate
			{
				_signalBus.TryFire(new IndicatorSignals.PushRequest(status: true, FilteredIndicatorType.MiniEventStart));
			}).SelectMany((Func<CalendarModel, IObservable<CalendarModel>>)PushOnceOnMeta)
			select _miniEventSettingsProvider.GetEvent(calendar.BalanceId) into minievent
			where minievent != null && !minievent.StartWindowShown && !string.IsNullOrEmpty(minievent.Promo.PromoView) && minievent.IsAnyContentAvailable.Value
			select minievent).Subscribe(delegate(MiniEvent minievent)
		{
			if (_miniEventsPromoPusherView == null)
			{
				_miniEventsPromoPusherView = _windowsManager.GetWindow(_startWindowID) as MiniEventsPromoPusherView;
			}
			_miniEventsPromoPusherView.TryShowPromos(minievent);
			_signalBus.TryFire(new IndicatorSignals.PushRequest(status: false, FilteredIndicatorType.MiniEventStart));
		}).AddTo(_compositeDisposable);
	}

	public void Dispose()
	{
		_compositeDisposable.Dispose();
	}

	private IObservable<CalendarModel> PushOnceOnMeta(CalendarModel calendarModel)
	{
		return from _ in _indicatorDisplayService.OnIndicatorPush(FilteredIndicatorType.MiniEventStart).FirstOrDefault((bool x) => x)
			where _calendarQueue.IsCalendarActive(calendarModel)
			select calendarModel;
	}
}
