using GreenT.HornyScapes.GameItems;
using GreenT.Types;
using Merge;
using StripClub.Model;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventMergeItemDispenser
{
	private readonly MiniEventMergeItemsDataCase _mergeItemsDataCase;

	private readonly MiniEventInventoryItemsDataCase _inventoryItemsDataCase;

	private readonly MiniEventPocketItemsDataCase _pocketItemsDataCase;

	private readonly GameItemConfigManager _gameItemConfigManager;

	public MiniEventMergeItemDispenser(MiniEventMergeItemsDataCase mergeItemsDataCase, MiniEventInventoryItemsDataCase inventoryItemsDataCase, MiniEventPocketItemsDataCase pocketItemsDataCase, GameItemConfigManager gameItemConfigManager)
	{
		_mergeItemsDataCase = mergeItemsDataCase;
		_inventoryItemsDataCase = inventoryItemsDataCase;
		_pocketItemsDataCase = pocketItemsDataCase;
		_gameItemConfigManager = gameItemConfigManager;
	}

	public void Set(GIData gameItem, MiniEventGameItemLocation location)
	{
		if (IsMiniEventCurrencyItem(gameItem))
		{
			CompositeIdentificator currencyId = GetCurrencyId(gameItem);
			switch (location)
			{
			case MiniEventGameItemLocation.Field:
				_mergeItemsDataCase.AddItem(gameItem, currencyId);
				break;
			case MiniEventGameItemLocation.Pocket:
				_pocketItemsDataCase.AddItem(gameItem, currencyId);
				break;
			case MiniEventGameItemLocation.Inventory:
				_inventoryItemsDataCase.AddItem(gameItem, currencyId);
				break;
			}
		}
	}

	public void Remove(GIData gameItem, MiniEventGameItemLocation location)
	{
		if (IsMiniEventCurrencyItem(gameItem))
		{
			CompositeIdentificator currencyId = GetCurrencyId(gameItem);
			switch (location)
			{
			case MiniEventGameItemLocation.Field:
				_mergeItemsDataCase.RemoveItem(gameItem, currencyId);
				break;
			case MiniEventGameItemLocation.Pocket:
				_pocketItemsDataCase.RemoveItem(gameItem, currencyId);
				break;
			case MiniEventGameItemLocation.Inventory:
				_inventoryItemsDataCase.RemoveItem(gameItem, currencyId);
				break;
			}
		}
	}

	private CompositeIdentificator GetCurrencyId(GIData gameItem)
	{
		return ((ModuleConfigs.Collect.CurrencyParams)_gameItemConfigManager.GetConfigOrNull(gameItem.Key).GetModule<ModuleConfigs.Collect>().Parametres).CompositeIdentificator;
	}

	private bool IsMiniEventCurrencyItem(GIData gameItem)
	{
		ModuleConfigs.Collect module = _gameItemConfigManager.GetConfigOrNull(gameItem.Key).GetModule<ModuleConfigs.Collect>();
		if (module == null)
		{
			return false;
		}
		if (!module.TryGetCurrencyParams(out var currencyParams))
		{
			return false;
		}
		if (currencyParams.CurrencyType != CurrencyType.MiniEvent)
		{
			return false;
		}
		return true;
	}
}
