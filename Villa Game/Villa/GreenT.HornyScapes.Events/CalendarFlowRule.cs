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
		_calendarManager = calendarManager;
		_timerTracker = timerTracker;
		_calendarQueue = calendarQueue;
		_migrateVersionData = migrateVersionData;
		_playerEventEnergyFactory = playerEventEnergyFactory;
	}

	public void Initialize()
	{
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
		IObservable<Unit> observable = collection.Select((CalendarModel model) => model.CalendarLoadingState.Where((CalendarModelLoadingState state) => state == CalendarModelLoadingState.Failed || state == CalendarModelLoadingState.Success).AsUnitObservable()).Merge();
		IObservable<CalendarModel> observable2 = (from calendar in collection
			where calendar.IsNotPassedCalendar && calendar.CalendarLoadingState.Value == CalendarModelLoadingState.None
			select _calendarQueue.OnCalendarAvailable(calendar)).Merge();
		(from _ in _calendarQueue.OnCalendarCountChange().Merge(observable)
			select (!_calendarManager.TryGetNextCalendar(_calendarQueue.ToList(), out var calendarModel)) ? null : calendarModel into calendar
			where calendar != null
			select calendar).Merge(observable2).Where(IsCalendarAvailable).Subscribe(delegate(CalendarModel calendar)
		{
			PrepareCalendarToStart(calendar);
		})
			.AddTo(_onUnlockStreams);
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
			IObservable<bool> right = _timerTracker.AddTrackTimer(comingSoonTimeStamp).OnTimeIsUp.Select((GenericTimer _) => true);
			IConnectableObservable<CalendarModel> connectableObservable = (from tuple in (from calendar in calendarModel.CalendarStrategy.Load(calendarModel).Take(1)
					select calendar != null && calendar.CalendarLoadingState.Value == CalendarModelLoadingState.Success).CombineLatest(right, (bool loaded, bool promoEnd) => (loaded: loaded, promoEnd: promoEnd))
				where tuple.loaded && tuple.promoEnd
				select tuple into _
				select calendarModel).Publish();
			connectableObservable.Subscribe(delegate(CalendarModel calendar)
			{
				StartCalendar(calendar);
			}, delegate(Exception exception)
			{
				throw exception.LogException();
			}).AddTo(_nextCalendarStreams);
			connectableObservable.Connect().AddTo(_nextCalendarStreams);
			(from _ in calendarModel.CalendarState.ObserveOn(Scheduler.MainThreadEndOfFrame).FirstOrDefault((EntityStatus _state) => _state == EntityStatus.Rewarded)
				select calendarModel).Subscribe(delegate(CalendarModel calendar)
			{
				StartLastChanceTimer(calendar);
			}, delegate(Exception exception)
			{
				throw exception.LogException();
			}).AddTo(_nextCalendarStreams);
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
		_timerTracker.AddTrackTimer(lastChanceTimeStamp).OnTimeIsUp.Take(1).Subscribe(delegate
		{
			EndCalendar(calendar);
		}, delegate(Exception exception)
		{
			throw exception.LogException();
		}).AddTo(_nextCalendarStreams);
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
