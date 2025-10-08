using System;
using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.MergeCore;
using Merge;
using StripClub.NewEvent.Model;
using StripClub.NewEvent.Save;

namespace StripClub.NewEvent.Data;

public class EventDataBuilder : ICalendarDataBuilder
{
	private EventDataCreateCase _createCase;

	private readonly ISaver _saver;

	private readonly MergeFieldFactory _fieldFactory;

	private readonly MergeFieldProvider _mergeFieldProvider;

	private readonly IContentFactory _contentFactory;

	private readonly EventSettingsProvider _eventSettingsProvider;

	public EventDataBuilder(ISaver saver, MergeFieldFactory fieldFactory, MergeFieldProvider mergeFieldProvider, IContentFactory contentFactory, EventSettingsProvider eventSettingsProvider)
	{
		_saver = saver;
		_fieldFactory = fieldFactory;
		_mergeFieldProvider = mergeFieldProvider;
		_contentFactory = contentFactory;
		_eventSettingsProvider = eventSettingsProvider;
	}

	public void CreateData(CalendarModel calendarModel, string saveKey)
	{
		if (_eventSettingsProvider.TryGetEvent(calendarModel.BalanceId, out var @event))
		{
			SetTargetEvent(@event);
			SetAllModules(saveKey);
			Create();
		}
	}

	private void SetTargetEvent(Event baseEvent)
	{
		_createCase = new EventDataCreateCase();
		_createCase.Event = baseEvent;
		CreateSaver();
	}

	private void CreateSaver()
	{
		_createCase.Saver = new EventDataSaver(_createCase.SaveKey);
	}

	private void SetAllModules(string saveKey)
	{
		AddWallet();
		AddMergeField(saveKey);
		AddPocketRepository();
		Migrate18_4To18_5.SetMigrate();
	}

	private void AddWallet()
	{
		int num = 0;
		_createCase.Wallet = new EventWallet();
		_createCase.ClearingInforation.Add(_createCase.Wallet);
		EventSimpleCurrencyFactory eventSimpleCurrencyFactory = new EventSimpleCurrencyFactory(_createCase.Saver, _createCase.Wallet);
		bool flag = Migrate18_4To18_5.NeedMigrate();
		foreach (EventCurrencyType value in Enum.GetValues(typeof(EventCurrencyType)))
		{
			eventSimpleCurrencyFactory.Create(value, flag ? Migrate18_4To18_5.GetCurrency(value) : num);
		}
	}

	private void AddMergeField(string saveKey)
	{
		_createCase.MergeField = _fieldFactory.Create(_createCase.Event.DefaultField, saveKey);
	}

	private void AddPocketRepository()
	{
		bool num = Migrate18_4To18_5.NeedMigrate();
		_createCase.PocketRepository = new PocketRepository();
		Queue<GIData> pocketEventItemsQueue = Controller<GreenT.HornyScapes.MergeCore.PocketController>.Instance.PocketEventItemsQueue;
		if (num && pocketEventItemsQueue != null)
		{
			_createCase.PocketRepository.SetQueue(pocketEventItemsQueue);
		}
		_createCase.Saver.Add(_createCase.PocketRepository);
		_createCase.ClearingInforation.Add(_createCase.PocketRepository);
	}

	private void Create()
	{
		_saver.Add(_createCase.Saver);
		_mergeFieldProvider.TryAdd(_createCase.MergeField);
		_createCase.Event.Data.Initialization(_createCase);
		_createCase = null;
	}
}
