using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.MergeCore.Inventory;
using GreenT.UI;
using Merge;
using Merge.Core.Inventory;
using Merge.Core.Masters;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

[MementoHolder]
public class InventoryController : Controller<InventoryController>, IGIDropCatcher, ISavableState, IGameTypeListener, Merge.IInputBlocker
{
	[Serializable]
	public class GeneralData : Merge.Data
	{
		[Serializable]
		public class ItemNode
		{
			[SerializeField]
			public GIData Item { get; set; }

			[SerializeField]
			public RefDateTime InputDate { get; set; }

			public ItemNode(GIData item, RefDateTime inputDate)
			{
				Item = item;
				InputDate = inputDate;
			}

			public ItemNode(GIData item)
			{
				Item = item;
				InputDate = RefDateTime.Now;
			}
		}

		[SerializeField]
		public List<ItemNode> StoredItems { get; set; } = new List<ItemNode>();


		[SerializeField]
		public int OpenedSlots { get; set; }

		public int FreeSlots => OpenedSlots - StoredItems.Count;
	}

	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		[SerializeField]
		private GeneralData data;

		public GeneralData Data => data;

		public Memento(InventoryController inventoryController)
			: base(inventoryController)
		{
			data = inventoryController.data;
		}
	}

	[SerializeField]
	private string[] ChestNames;

	private GeneralData data;

	[Inject]
	private IWindowsManager windowsManager;

	[Inject]
	private InventorySettingsProvider inventorySettingsProvider;

	[Inject]
	private GameItemConfigManager gameItemConfigManager;

	private CoreWindow coreWindow;

	private MergeInventoryWindow window;

	private const string key = "InventoryController";

	public bool HasEmptySlot => data.OpenedSlots != data.StoredItems.Count;

	public int SlotsTotal => data.OpenedSlots;

	public GameItemController Field => Controller<GameItemController>.Instance;

	public IEnumerable<GIData> StoredItems => data.StoredItems.Select((GeneralData.ItemNode _node) => _node.Item);

	public bool BlocksClick => window.IsOpened;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public event Action<int> OnSlotBuy;

	public event Action<GIData> OnItemAdded;

	public event Action<GIData> OnItemRemoved;

	public override void Preload()
	{
		base.Preload();
		data = new GeneralData();
		data.OpenedSlots = inventorySettingsProvider.StartOpenSlotsCount;
		coreWindow = windowsManager.Get<CoreWindow>();
		window = windowsManager.Get<MergeInventoryWindow>();
		coreWindow.uiButton.onClick.AddListener(AtMainButtonClick);
	}

	public override void Init()
	{
		window.Init(inventorySettingsProvider, data);
		window.OnItemClick += PullItemFromInventory;
		window.OnSlotBuy += delegate
		{
			this.OnSlotBuy?.Invoke(data.OpenedSlots);
		};
		UpdateCounter();
		window.OnSlotBuy += UpdateCounter;
	}

	protected override void OnDestroy()
	{
		window.OnSlotBuy -= UpdateCounter;
		base.OnDestroy();
	}

	private void UpdateCounter()
	{
		coreWindow.UpdateInventoryCounter(data.StoredItems.Count, data.OpenedSlots);
	}

	void IGameTypeListener.SetGameType(PlayType playType)
	{
		coreWindow.uiButton.SetActive(playType == PlayType.story);
	}

	void IGIDropCatcher.CatchDrop(GameItem item)
	{
		string[] chestNames = ChestNames;
		foreach (string text in chestNames)
		{
			if (item.Key.Collection == text && item.Data.Modules[0] is ModuleDatas.Chest { IsOpeningNow: not false })
			{
				item.DoFlyTo(TileController.GetPosition(item.Coordinates));
				return;
			}
		}
		if (item.Config.GameItemName == "Battery" && item.Data.Modules[0] is ModuleDatas.Tesla && (item.Data.Modules[0] as ModuleDatas.Tesla).Activated)
		{
			item.DoFlyTo(TileController.GetPosition(item.Coordinates));
			return;
		}
		GIData giData = item.Data.Copy();
		Controller<GameItemController>.Instance.RemoveItem(item);
		AddItem(giData);
	}

	bool IGIDropCatcher.IsCatchesDrop(GameItem item)
	{
		GIBox.Bubble box;
		if (coreWindow.uiButton.IsPointerOver && HasEmptySlot)
		{
			return !item.TryGetBox<GIBox.Bubble>(out box);
		}
		return false;
	}

	public string UniqueKey()
	{
		return "InventoryController";
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		MigrationToBattlePass.Migrate(memento2.Data.StoredItems);
		foreach (GeneralData.ItemNode storedItem in memento2.Data.StoredItems)
		{
			if (IsItemExistInConfig(storedItem))
			{
				data.StoredItems.Add(storedItem);
			}
		}
		data.OpenedSlots = memento2.Data.OpenedSlots;
	}

	private bool IsItemExistInConfig(GeneralData.ItemNode itemNode)
	{
		return gameItemConfigManager.GetConfigOrNull(itemNode.Item.Key) != null;
	}

	public void AddItem(GIData giData)
	{
		data.StoredItems.Add(new GeneralData.ItemNode(giData));
		this.OnItemAdded?.Invoke(giData);
		UpdateCounter();
	}

	public void RemoveItemsFromInventory(IEnumerable<GIData> items)
	{
		foreach (GeneralData.ItemNode item in data.StoredItems.Where((GeneralData.ItemNode node) => items.Contains(node.Item)).ToList())
		{
			data.StoredItems.Remove(item);
		}
		UpdateCounter();
		window.Repaint();
	}

	private void AtBeginDrag(GameItem gi)
	{
		gi.TryGetBox<GIBox.Bubble>(out var _);
	}

	private void AtMainButtonClick()
	{
		window.Repaint();
	}

	private void PullItemFromInventory(GeneralData.ItemNode node)
	{
		if (Field.TryGetFirstEmptyPoint(out var pnt))
		{
			GIData item = node.Item;
			item.Coordinates = pnt;
			List<ITimerData> list = (from x in item.Modules
				where x is ITimerData
				select x as ITimerData).ToList();
			TimeSpan timeSpan = TimeMaster.Now - node.InputDate.Value;
			for (int i = 0; i < list.Count; i++)
			{
				list[i].MainTimer.StartTime += timeSpan;
			}
			this.OnItemRemoved?.Invoke(item);
			GameItem pulled = Field.TakeItemFromSomethere(item);
			window.DoFlyObject(pulled, node);
			data.StoredItems.Remove(node);
			window.Repaint();
			UpdateCounter();
		}
	}
}
