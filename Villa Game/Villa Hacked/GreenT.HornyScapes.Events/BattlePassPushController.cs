using System;
using System.Collections.Generic;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.DisplayStrategy;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.IndicatorAdapter;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.UI;
using JetBrains.Annotations;
using Merge.Meta.RoomObjects;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Events;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
public class BattlePassPushController : IInitializable, IDisposable
{
	private readonly CalendarQueue _calendarQueue;

	private readonly WindowID _startWindowID;

	private readonly IWindowsManager _windowsManager;

	private readonly IndicatorDisplayService _indicatorDisplayService;

	private readonly SignalBus _signalBus;

	private readonly BattlePassSettingsProvider _battlePassSettingsProvider;

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	public BattlePassPushController(CalendarQueue calendarQueue, WindowID startWindowID, BattlePassSettingsProvider battlePassSettingsProvider, IndicatorDisplayService indicatorDisplayService, SignalBus signalBus, IWindowsManager windowsManager)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_signalBus = signalBus;
		_windowsManager = windowsManager;
		_calendarQueue = calendarQueue;
		_startWindowID = startWindowID;
		_indicatorDisplayService = indicatorDisplayService;
		_battlePassSettingsProvider = battlePassSettingsProvider;
	}

	public void Initialize()
	{
		_compositeDisposable.Clear();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<BattlePass>(Observable.Where<BattlePass>(Observable.Select<CalendarModel, BattlePass>(Observable.SelectMany<CalendarModel, CalendarModel>(Observable.Do<CalendarModel>(Observable.DelayFrame<CalendarModel>(Observable.Where<CalendarModel>(_calendarQueue.OnCalendarStateChange(EventStructureType.BattlePass), (Func<CalendarModel, bool>)((CalendarModel calendar) => calendar.CalendarState.Value == EntityStatus.InProgress && IsFirstBP(calendar))), 1, (FrameCountType)0), (Action<CalendarModel>)delegate
		{
			_signalBus.TryFire<IndicatorSignals.PushRequest>(new IndicatorSignals.PushRequest(status: true, FilteredIndicatorType.BattlePassStart));
		}), (Func<CalendarModel, IObservable<CalendarModel>>)PushOnceOnMeta), (Func<CalendarModel, BattlePass>)((CalendarModel calendar) => _battlePassSettingsProvider.GetBattlePass(calendar.BalanceId))), (Func<BattlePass, bool>)((BattlePass battlePass) => battlePass != null && !battlePass.Data.StartData.StartWindowShown)), (Action<BattlePass>)delegate(BattlePass battlePass)
		{
			_windowsManager.GetWindow(_startWindowID).Open();
			battlePass.Data.StartData.SetFirstTimePushed();
			battlePass.Data.StartData.SetStartedBattlePassProgress();
			_signalBus.TryFire<IndicatorSignals.PushRequest>(new IndicatorSignals.PushRequest(status: false, FilteredIndicatorType.BattlePassStart));
		}), (ICollection<IDisposable>)_compositeDisposable);
	}

	private bool IsFirstBP(CalendarModel calendar)
	{
		if (_battlePassSettingsProvider.TryGetBattlePass(calendar.BalanceId, out var battlePass))
		{
			return !battlePass.Data.StartData.StartWindowShown;
		}
		return false;
	}

	private IObservable<CalendarModel> PushOnceOnMeta(CalendarModel calendarModel)
	{
		return Observable.Select<bool, CalendarModel>(Observable.Where<bool>(Observable.FirstOrDefault<bool>(_indicatorDisplayService.OnIndicatorPush(FilteredIndicatorType.BattlePassStart), (Func<bool, bool>)((bool x) => x)), (Func<bool, bool>)((bool _) => _calendarQueue.IsCalendarActive(calendarModel))), (Func<bool, CalendarModel>)((bool _) => calendarModel));
	}

	public void Dispose()
	{
		_compositeDisposable.Dispose();
	}
}
