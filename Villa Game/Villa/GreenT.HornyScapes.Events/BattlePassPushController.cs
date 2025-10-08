using System;
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
		(from calendar in (from calendar in _calendarQueue.OnCalendarStateChange(EventStructureType.BattlePass)
				where calendar.CalendarState.Value == EntityStatus.InProgress && IsFirstBP(calendar)
				select calendar).DelayFrame(1).Do(delegate
			{
				_signalBus.TryFire(new IndicatorSignals.PushRequest(status: true, FilteredIndicatorType.BattlePassStart));
			}).SelectMany((Func<CalendarModel, IObservable<CalendarModel>>)PushOnceOnMeta)
			select _battlePassSettingsProvider.GetBattlePass(calendar.BalanceId) into battlePass
			where battlePass != null && !battlePass.Data.StartData.StartWindowShown
			select battlePass).Subscribe(delegate(BattlePass battlePass)
		{
			_windowsManager.GetWindow(_startWindowID).Open();
			battlePass.Data.StartData.SetFirstTimePushed();
			battlePass.Data.StartData.SetStartedBattlePassProgress();
			_signalBus.TryFire(new IndicatorSignals.PushRequest(status: false, FilteredIndicatorType.BattlePassStart));
		}).AddTo(_compositeDisposable);
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
		return from _ in _indicatorDisplayService.OnIndicatorPush(FilteredIndicatorType.BattlePassStart).FirstOrDefault((bool x) => x)
			where _calendarQueue.IsCalendarActive(calendarModel)
			select calendarModel;
	}

	public void Dispose()
	{
		_compositeDisposable.Dispose();
	}
}
