using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using Merge.Meta.RoomObjects;
using StripClub.UI;
using UniRx;

namespace GreenT.HornyScapes.Events;

public class CalendarFlowRule : IDisposable
{
	private CompositeDisposable _onUnlockStreams = new CompositeDisposable();

	private CompositeDisposable _nextCalendarStreams = new CompositeDisposable();

	private readonly TimerTracker _timerTracker;

	private readonly CalendarQueue _calendarQueue;

	private readonly CalendarManager _calendarManager;

	private readonly MigrateVersionData _migrateVersionData;

	private readonly PlayerEventEnergyFactory _playerEventEnergyFactory;

	private readonly HashSet<int> _preparedCalendarIds = new HashSet<int>(16);

	private CalendarFlowRule(CalendarManager calendarManager, TimerTracker timerTracker, CalendarQueue calendarQueue, MigrateVersionData migrateVersionData, PlayerEventEnergyFactory playerEventEnergyFactory)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_calendarManager = calendarManager;
		_timerTracker = timerTracker;
		_calendarQueue = calendarQueue;
		_migrateVersionData = migrateVersionData;
		_playerEventEnergyFactory = playerEventEnergyFactory;
	}

	public void Initialize()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		_onUnlockStreams.Dispose();
		_nextCalendarStreams.Dispose();
		_onUnlockStreams = new CompositeDisposable();
		_nextCalendarStreams = new CompositeDisposable();
		_preparedCalendarIds.Clear();
		RestoreStartedAndNotPassedCalendar();
		SubscribeOnUnlock();
	}

	private void RestoreStartedAndNotPassedCalendar()
	{
		RestoreCalendarIfNecessary(EventStructureType.Mini);
		RestoreCalendarIfNecessary(EventStructureType.Event);
		RestoreCalendarIfNecessary(EventStructureType.BattlePass);
		RestoreCalendarIfNecessary(EventStructureType.Sellout);
	}

	private void RestoreCalendarIfNecessary(EventStructureType type)
	{
		if (!RestoreCalendar(type, (CalendarModel calendar) => calendar.WasStarted && calendar.IsNotPassedCalendar))
		{
			RestoreCalendar(type, (CalendarModel calendar) => calendar.IsNotPassedCalendar);
		}
	}

	private bool RestoreCalendar(EventStructureType type, Func<CalendarModel, bool> rule)
	{
		CalendarModel calendarModel = (from calendar in _calendarManager.Collection.Where((CalendarModel calendar) => calendar.Locker.IsOpen.Value).ToList()
			where calendar.EventType == type
			select calendar).FirstOrDefault(rule);
		bool num = calendarModel != null;
		if (num)
		{
			PrepareCalendarToStart(calendarModel);
		}
		return num;
	}

	private void Migration(CalendarModel calendarModel)
	{
		if (calendarModel is PeriodicCalendar && !_migrateVersionData.IsMigrate && calendarModel.EventType == EventStructureType.Event)
		{
			calendarModel.CalendarStrategy.Clean(calendarModel);
			_migrateVersionData.IsMigrate = true;
		}
	}

	private void SubscribeOnUnlock()
	{
		IEnumerable<CalendarModel> collection = _calendarManager.Collection;
		IObservable<Unit> observable = Observable.Merge<Unit>(collection.Select((CalendarModel model) => Observable.AsUnitObservable<CalendarModelLoadingState>(Observable.Where<CalendarModelLoadingState>((IObservable<CalendarModelLoadingState>)model.CalendarLoadingState, (Func<CalendarModelLoadingState, bool>)((CalendarModelLoadingState state) => state == CalendarModelLoadingState.Failed || state == CalendarModelLoadingState.Success)))));
		IObservable<CalendarModel> observable2 = Observable.Merge<CalendarModel>(from calendar in collection
			where calendar.IsNotPassedCalendar && calendar.CalendarLoadingState.Value == CalendarModelLoadingState.None
			select _calendarQueue.OnCalendarAvailable(calendar));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CalendarModel>(Observable.Where<CalendarModel>(Observable.Merge<CalendarModel>(Observable.Where<CalendarModel>(Observable.Select<Unit, CalendarModel>(Observable.Merge<Unit>(_calendarQueue.OnCalendarCountChange(), new IObservable<Unit>[1] { observable }), (Func<Unit, CalendarModel>)((Unit _) => (!_calendarManager.TryGetNextCalendar(_calendarQueue.ToList(), out var calendarModel)) ? null : calendarModel)), (Func<CalendarModel, bool>)((CalendarModel calendar) => calendar != null)), new IObservable<CalendarModel>[1] { observable2 }), (Func<CalendarModel, bool>)IsCalendarAvailable), (Action<CalendarModel>)delegate(CalendarModel calendar)
		{
			PrepareCalendarToStart(calendar);
		}), (ICollection<IDisposable>)_onUnlockStreams);
	}

	private bool IsCalendarAvailable(CalendarModel prepared)
	{
		if (prepared.CalendarLoadingState.Value != 0)
		{
			return false;
		}
		if (_calendarManager.IsAnyCalendarLoading())
		{
			return false;
		}
		return _calendarManager.IsCalendarAvailable(_calendarQueue.ToList(), prepared);
	}

	private void PrepareCalendarToStart(CalendarModel calendarModel)
	{
		if (!_preparedCalendarIds.Contains(calendarModel.UniqID))
		{
			_preparedCalendarIds.Add(calendarModel.UniqID);
			Migration(calendarModel);
			long comingSoonTimeStamp = calendarModel.ComingSoonTimeStamp;
			IObservable<bool> observable = Observable.Select<GenericTimer, bool>(_timerTracker.AddTrackTimer(comingSoonTimeStamp).OnTimeIsUp, (Func<GenericTimer, bool>)((GenericTimer _) => true));
			IConnectableObservable<CalendarModel> obj = Observable.Publish<CalendarModel>(Observable.Select<(bool, bool), CalendarModel>(Observable.Where<(bool, bool)>(Observable.CombineLatest<bool, bool, (bool, bool)>(Observable.Select<CalendarModel, bool>(Observable.Take<CalendarModel>(calendarModel.CalendarStrategy.Load(calendarModel), 1), (Func<CalendarModel, bool>)((CalendarModel calendar) => calendar != null && calendar.CalendarLoadingState.Value == CalendarModelLoadingState.Success)), observable, (Func<bool, bool, (bool, bool)>)((bool loaded, bool promoEnd) => (loaded: loaded, promoEnd: promoEnd))), (Func<(bool, bool), bool>)(((bool loaded, bool promoEnd) tuple) => tuple.loaded && tuple.promoEnd)), (Func<(bool, bool), CalendarModel>)(((bool loaded, bool promoEnd) _) => calendarModel)));
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CalendarModel>((IObservable<CalendarModel>)obj, (Action<CalendarModel>)delegate(CalendarModel calendar)
			{
				StartCalendar(calendar);
			}, (Action<Exception>)delegate(Exception exception)
			{
				throw exception.LogException();
			}), (ICollection<IDisposable>)_nextCalendarStreams);
			DisposableExtensions.AddTo<IDisposable>(obj.Connect(), (ICollection<IDisposable>)_nextCalendarStreams);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CalendarModel>(Observable.Select<EntityStatus, CalendarModel>(Observable.FirstOrDefault<EntityStatus>(Observable.ObserveOn<EntityStatus>((IObservable<EntityStatus>)calendarModel.CalendarState, Scheduler.MainThreadEndOfFrame), (Func<EntityStatus, bool>)((EntityStatus _state) => _state == EntityStatus.Rewarded)), (Func<EntityStatus, CalendarModel>)((EntityStatus _) => calendarModel)), (Action<CalendarModel>)delegate(CalendarModel calendar)
			{
				StartLastChanceTimer(calendar);
			}, (Action<Exception>)delegate(Exception exception)
			{
				throw exception.LogException();
			}), (ICollection<IDisposable>)_nextCalendarStreams);
		}
	}

	private void StartCalendar(CalendarModel calendarModel)
	{
		calendarModel.Launch();
		try
		{
			TEMP_FixEventEnergy_TEMP(calendarModel);
			string saveKey = MigrateTo12Version(calendarModel);
			calendarModel.CalendarStrategy.Build(calendarModel, saveKey);
			calendarModel.IsLoading = false;
			calendarModel.CalendarStrategy.Dispense(calendarModel.BalanceId);
			_calendarQueue.Add(calendarModel);
		}
		catch (Exception)
		{
			if (calendarModel != null)
			{
				_ = $"{calendarModel.EventType}_{calendarModel.EventMapper.ID}";
			}
		}
	}

	private void StartLastChanceTimer(CalendarModel calendar)
	{
		if (calendar.CalendarState.Value != EntityStatus.Rewarded)
		{
			return;
		}
		long lastChanceTimeStamp = calendar.LastChanceTimeStamp;
		if (lastChanceTimeStamp == 0L)
		{
			EndCalendar(calendar);
			return;
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<GenericTimer>(Observable.Take<GenericTimer>(_timerTracker.AddTrackTimer(lastChanceTimeStamp).OnTimeIsUp, 1), (Action<GenericTimer>)delegate
		{
			EndCalendar(calendar);
		}, (Action<Exception>)delegate(Exception exception)
		{
			throw exception.LogException();
		}), (ICollection<IDisposable>)_nextCalendarStreams);
	}

	private void TEMP_FixEventEnergy_TEMP(CalendarModel calendarModel)
	{
		if (calendarModel.EventType == EventStructureType.Event && !calendarModel.EventEnergyResetToStart)
		{
			_playerEventEnergyFactory.CreateEventEnergy();
			calendarModel.EventEnergyResetToStart = true;
		}
	}

	private string MigrateTo12Version(CalendarModel calendarModel)
	{
		if (!(calendarModel is NoviceCalendar))
		{
			return $"{1}_{calendarModel.UniqID}";
		}
		return $"{1}";
	}

	private void EndCalendar(CalendarModel calendarModel)
	{
		try
		{
			if (!calendarModel.WasEnded)
			{
				calendarModel.CalendarStrategy.Clean(calendarModel);
				calendarModel.WasEnded = true;
				if (_calendarQueue.IsCalendarActive(calendarModel))
				{
					_calendarQueue.Remove(calendarModel);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public void Dispose()
	{
		_nextCalendarStreams.Dispose();
		_onUnlockStreams.Dispose();
		_preparedCalendarIds.Clear();
	}
}
