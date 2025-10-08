using System;
using GreenT.Data;
using GreenT.HornyScapes.Bank;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.MergeCore;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.NewEvent.Data;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Events;

public class EventDataCleaner : ICalendarDataCleaner
{
	private readonly ISaver _saver;

	private readonly DataCleanerManager _dataCleanerManager;

	private readonly FieldCleaner _fieldCleaner;

	private readonly EventSettingsProvider _eventSettingsProvider;

	private readonly ICurrencyProcessor _currencyProcessor;

	private readonly SpendEventEnergyTracker _spendEventEnergyTracker;

	private readonly MergePointsController _mergePointsController;

	private readonly ResetAfterBundleLotController _resetAfterBundleLotController;

	private readonly EventRewardTracker _rewardTracker;

	private readonly Subject<Event> _onResetRequest = new Subject<Event>();

	public IObservable<Event> OnResetRequest => _onResetRequest;

	public EventDataCleaner(ISaver saver, DataCleanerManager dataCleanerManager, FieldCleaner fieldCleaner, EventSettingsProvider eventSettingsProvider, ICurrencyProcessor currencyProcessor, SpendEventEnergyTracker spendEventEnergyTracker, MergePointsController mergePointsController, ResetAfterBundleLotController resetAfterBundleLotController, EventRewardTracker rewardTracker)
	{
		_saver = saver;
		_dataCleanerManager = dataCleanerManager;
		_fieldCleaner = fieldCleaner;
		_eventSettingsProvider = eventSettingsProvider;
		_currencyProcessor = currencyProcessor;
		_spendEventEnergyTracker = spendEventEnergyTracker;
		_mergePointsController = mergePointsController;
		_resetAfterBundleLotController = resetAfterBundleLotController;
		_rewardTracker = rewardTracker;
	}

	public void CleanData(CalendarModel calendar)
	{
		if (calendar == null)
		{
			Debug.LogError(new NullReferenceException("[EventDataCleaner] CalendarModel is null").LogException());
			return;
		}
		try
		{
			RestoreEventEnergy();
			RestoreEventCurrency();
			CleanGlobalData();
			TryClearMergePoints();
			TryResetBundle(calendar);
			if (_eventSettingsProvider.TryGetEvent(calendar.BalanceId, out var @event))
			{
				CleanEventData(@event);
			}
			ClearRewardTracker();
		}
		catch (Exception exception)
		{
			exception.LogException();
		}
	}

	private void TryResetBundle(CalendarModel calendar)
	{
		if (calendar.EventMapper != null)
		{
			int iD = calendar.EventMapper.ID;
			_resetAfterBundleLotController?.TryClear(EventStructureType.Event, iD);
		}
	}

	private void TryClearMergePoints()
	{
		_mergePointsController.TryRemoveEventPoints(EventStructureType.Event);
	}

	private void CleanGlobalData()
	{
		foreach (IDataCleaner item in _dataCleanerManager.Collection)
		{
			try
			{
				item.ClearData();
			}
			catch (Exception exception)
			{
				exception.LogException();
			}
		}
	}

	private void CleanEventData(Event eventSettings)
	{
		Migrate18_4To18_5.SetMigrate();
		if (eventSettings?.Data == null)
		{
			return;
		}
		EventDataCase data = eventSettings.Data;
		CleanMergeField(data.MergeField);
		foreach (EventReward eventReward in eventSettings.GetEventRewards())
		{
			eventReward.SetBlocked();
		}
		_saver.Delete(data.Saver);
		_saver.Delete(eventSettings);
		_onResetRequest?.OnNext(eventSettings);
		foreach (IDisposableEventInformation item in data.DisposableInforation)
		{
			try
			{
				item.Dispose();
			}
			catch (Exception exception)
			{
				exception.LogException();
			}
		}
	}

	private void CleanMergeField(GreenT.HornyScapes.MergeCore.MergeField field)
	{
		if (field != null)
		{
			_fieldCleaner.Clean(field);
		}
	}

	private void RestoreEventEnergy()
	{
		_currencyProcessor.TryReset(CurrencyType.EventEnergy);
		_spendEventEnergyTracker.ResetData(string.Empty);
	}

	private void RestoreEventCurrency()
	{
		_currencyProcessor.TryReset(CurrencyType.Event);
		_currencyProcessor.TryReset(CurrencyType.EventXP);
	}

	private void ClearRewardTracker()
	{
		_rewardTracker.Clear();
	}
}
