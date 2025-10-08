using System;
using System.Collections.Generic;
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<MiniEvent>(Observable.Where<MiniEvent>(Observable.Select<CalendarModel, MiniEvent>(Observable.SelectMany<CalendarModel, CalendarModel>(Observable.Do<CalendarModel>(Observable.DelayFrame<CalendarModel>(Observable.Where<CalendarModel>(_calendarQueue.OnCalendarStateChange(EventStructureType.Mini), (Func<CalendarModel, bool>)((CalendarModel calendar) => calendar.CalendarState.Value == EntityStatus.InProgress)), 1, (FrameCountType)0), (Action<CalendarModel>)delegate
		{
			_signalBus.TryFire<IndicatorSignals.PushRequest>(new IndicatorSignals.PushRequest(status: true, FilteredIndicatorType.MiniEventStart));
		}), (Func<CalendarModel, IObservable<CalendarModel>>)PushOnceOnMeta), (Func<CalendarModel, MiniEvent>)((CalendarModel calendar) => _miniEventSettingsProvider.GetEvent(calendar.BalanceId))), (Func<MiniEvent, bool>)((MiniEvent minievent) => minievent != null && !minievent.StartWindowShown && !string.IsNullOrEmpty(minievent.Promo.PromoView) && minievent.IsAnyContentAvailable.Value)), (Action<MiniEvent>)delegate(MiniEvent minievent)
		{
			if (_miniEventsPromoPusherView == null)
			{
				_miniEventsPromoPusherView = _windowsManager.GetWindow(_startWindowID) as MiniEventsPromoPusherView;
			}
			_miniEventsPromoPusherView.TryShowPromos(minievent);
			_signalBus.TryFire<IndicatorSignals.PushRequest>(new IndicatorSignals.PushRequest(status: false, FilteredIndicatorType.MiniEventStart));
		}), (ICollection<IDisposable>)_compositeDisposable);
	}

	public void Dispose()
	{
		_compositeDisposable.Dispose();
	}

	private IObservable<CalendarModel> PushOnceOnMeta(CalendarModel calendarModel)
	{
		return Observable.Select<bool, CalendarModel>(Observable.Where<bool>(Observable.FirstOrDefault<bool>(_indicatorDisplayService.OnIndicatorPush(FilteredIndicatorType.MiniEventStart), (Func<bool, bool>)((bool x) => x)), (Func<bool, bool>)((bool _) => _calendarQueue.IsCalendarActive(calendarModel))), (Func<bool, CalendarModel>)((bool _) => calendarModel));
	}
}
