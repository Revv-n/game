using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.MiniEvents;
using Merge;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class MiniEventInventoryController : MiniEventMergeController<MiniEventInventoryController>
{
	private GameItemConfigManager _gameItemConfigManager;

	[Inject]
	public void Init(GameItemConfigManager gameItemConfigManager)
	{
		_gameItemConfigManager = gameItemConfigManager;
	}

	public override void Init()
	{
		base.Init();
		InventoryController inventoryController = Controller<InventoryController>.Instance;
		IEnumerable<GIData> items = inventoryController.StoredItems.Where((GIData item) => IsMiniEventCurrencyItem(_gameItemConfigManager.GetConfigOrNull(item.Key)));
		inventoryController.RemoveItemsFromInventory(items);
		Observable.FromEvent(delegate(Action<GIData> handler)
		{
			inventoryController.OnItemAdded += handler;
		}, delegate(Action<GIData> handler)
		{
			inventoryController.OnItemRemoved -= handler;
		}).Subscribe(OnItemAdded).AddTo(_trackStream);
		Observable.FromEvent(delegate(Action<GIData> handler)
		{
			inventoryController.OnItemRemoved += handler;
		}, delegate(Action<GIData> handler)
		{
			inventoryController.OnItemRemoved -= handler;
		}).Subscribe(OnItemRemoved).AddTo(_trackStream);
	}

	public override void RestoreItems(IEnumerable<GIData> items)
	{
		InventoryController instance = Controller<InventoryController>.Instance;
		foreach (GIData item in items)
		{
			instance.AddItem(item);
		}
	}

	private void OnItemAdded(GIData item)
	{
		_mergeItemDispenser.Set(item, MiniEventGameItemLocation.Inventory);
	}

	private void OnItemRemoved(GIData item)
	{
		_mergeItemDispenser.Remove(item, MiniEventGameItemLocation.Inventory);
	}
}
