using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Bank;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.MergeCore;
using GreenT.Types;
using Merge;
using StripClub.Model;
using StripClub.NewEvent.Data;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventFieldCleaner
{
	private readonly CalendarQueue _calendarQueue;

	private readonly EventProvider _eventProvider;

	private readonly MergePointsController _mergePointsController;

	private readonly ResetAfterBundleLotController _resetAfterBundleLotController;

	private readonly GameItemConfigManager _gameItemConfigManager;

	private MiniEvent _currentMiniEvent;

	public MiniEventFieldCleaner(CalendarQueue calendarQueue, EventProvider eventProvider, MergePointsController mergePointsController, ResetAfterBundleLotController resetAfterBundleLotController, GameItemConfigManager gameItemConfigManager)
	{
		_calendarQueue = calendarQueue;
		_eventProvider = eventProvider;
		_mergePointsController = mergePointsController;
		_resetAfterBundleLotController = resetAfterBundleLotController;
		_gameItemConfigManager = gameItemConfigManager;
	}

	public void CleanData(MiniEvent miniEvent)
	{
		_currentMiniEvent = miniEvent;
		if (_calendarQueue.HasActiveCalendar(EventStructureType.Event))
		{
			CleanEventPocketData();
			CleanEventFieldData();
		}
		TryClearMergePoints(miniEvent);
		TryResetBundle(miniEvent);
		CleanFieldData();
		CleanPocketData();
		CleanInventoryData();
	}

	private void TryResetBundle(MiniEvent miniEvent)
	{
		_resetAfterBundleLotController.TryClear(EventStructureType.Mini, miniEvent.EventId);
	}

	private void TryClearMergePoints(MiniEvent miniEvent)
	{
		_mergePointsController.TryRemoveEventPoints(EventStructureType.Mini, miniEvent.CurrencyIdentificator);
	}

	private void CleanPocketData()
	{
		GreenT.HornyScapes.MergeCore.PocketController instance = Controller<GreenT.HornyScapes.MergeCore.PocketController>.Instance;
		if (instance.PocketItemsQueue.Any())
		{
			IEnumerable<GIData> itemsToClean = instance.PocketItemsQueue.Where((GIData data) => IsMiniEventCurrencyItem(_gameItemConfigManager.GetConfigOrNull(data.Key)));
			instance.RebaseMain(instance.PocketItemsQueue.Where((GIData item) => !itemsToClean.Contains(item)).ToList());
		}
	}

	private void CleanEventPocketData()
	{
		GreenT.HornyScapes.MergeCore.PocketController instance = Controller<GreenT.HornyScapes.MergeCore.PocketController>.Instance;
		Queue<GIData> queue = _eventProvider.CurrentCalendarProperty.Value.@event.Data.PocketRepository.Queue;
		if (queue.Any())
		{
			IEnumerable<GIData> itemsToClean = queue.Where((GIData data) => IsMiniEventCurrencyItem(_gameItemConfigManager.GetConfigOrNull(data.Key)));
			List<GIData> newCollection = queue.Where((GIData item) => !itemsToClean.Contains(item)).ToList();
			instance.RebaseEvent(newCollection);
		}
	}

	private void CleanInventoryData()
	{
		InventoryController instance = Controller<InventoryController>.Instance;
		IEnumerable<GIData> items = instance.StoredItems.Where((GIData data) => IsMiniEventCurrencyItem(_gameItemConfigManager.GetConfigOrNull(data.Key)));
		instance.RemoveItemsFromInventory(items);
	}

	private void CleanFieldData()
	{
		GameItemController instance = Controller<GameItemController>.Instance;
		instance.OpenField(ContentType.Main);
		IEnumerable<GameItem> items = from item in instance.CurrentField.Field.Objects
			where item.Config.HasModule<ModuleConfigs.Collect>()
			where IsMiniEventCurrencyItem(item.Config)
			select item;
		instance.RemoveItems(items);
	}

	private void CleanEventFieldData()
	{
		GameItemController instance = Controller<GameItemController>.Instance;
		instance.OpenField(ContentType.Event);
		IEnumerable<GameItem> items = from item in instance.CurrentField.Field.Objects
			where item.Config.HasModule<ModuleConfigs.Collect>()
			where IsMiniEventCurrencyItem(item.Config)
			select item;
		instance.RemoveItems(items);
	}

	private bool IsMiniEventCurrencyItem(GIConfig config)
	{
		ModuleConfigs.Collect module = config.GetModule<ModuleConfigs.Collect>();
		if (module == null)
		{
			return false;
		}
		if (!module.TryGetCurrencyParams(out var currencyParams))
		{
			return false;
		}
		if (currencyParams.CurrencyType != CurrencyType.MiniEvent || currencyParams.CompositeIdentificator != _currentMiniEvent.CurrencyIdentificator)
		{
			return false;
		}
		return true;
	}
}
