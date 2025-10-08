using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.MergeCore.Collection;
using Merge;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

[MementoHolder]
public class CollectionController : Controller<CollectionController>, ISavableState
{
	[Serializable]
	public class CollectionMemento : Memento
	{
		[Serializable]
		public class CGroup
		{
			[SerializeField]
			private string collectionId;

			[SerializeField]
			private List<CItem> items = new List<CItem>();

			public string CollectionID
			{
				get
				{
					return collectionId;
				}
				set
				{
					collectionId = value;
				}
			}

			public List<CItem> Items
			{
				get
				{
					return items;
				}
				set
				{
					items = value;
				}
			}

			public CGroup(string collectionId)
			{
				this.collectionId = collectionId;
			}
		}

		[Serializable]
		public class CItem
		{
			[SerializeField]
			private GIKey key;

			[SerializeField]
			private bool opened;

			public GIKey Key
			{
				get
				{
					return key;
				}
				set
				{
					key = value;
				}
			}

			public bool Opened
			{
				get
				{
					return opened;
				}
				set
				{
					opened = value;
				}
			}

			public CItem(GIKey key)
			{
				this.key = key;
			}

			public CItem(GIKey key, bool opened)
				: this(key)
			{
				this.opened = opened;
			}

			public void Init(CItem other)
			{
				Opened = other.Opened;
			}
		}

		[SerializeField]
		private Dictionary<string, CGroup> nodes = new Dictionary<string, CGroup>();

		public Dictionary<string, CGroup> Groups
		{
			get
			{
				return nodes;
			}
			set
			{
				nodes = value;
			}
		}

		public CItem this[GIKey key] => nodes[key.Collection].Items[key.ID - 1];

		public CollectionMemento(CollectionController collectionController)
			: base(collectionController)
		{
		}
	}

	private class CItemKeyComparer : IEqualityComparer<CollectionMemento.CItem>, IComparer<CollectionMemento.CItem>
	{
		public bool Equals(CollectionMemento.CItem x, CollectionMemento.CItem y)
		{
			return x.Key.Equals(y.Key);
		}

		public int GetHashCode(CollectionMemento.CItem obj)
		{
			return obj.Key.GetHashCode();
		}

		public int Compare(CollectionMemento.CItem x, CollectionMemento.CItem y)
		{
			if (x.Opened && !y.Opened)
			{
				return 1;
			}
			if (x.Opened && y.Opened && x.Key.ID > y.Key.ID)
			{
				return x.Key.ID.CompareTo(y.Key.ID);
			}
			if (!x.Opened && y.Opened)
			{
				return -1;
			}
			return 0;
		}
	}

	[SerializeField]
	private CollectionWindow collectionWindow;

	private CollectionMemento data;

	private GameItemConfigManager gameItemConfigManager;

	private GameItem _currentSelectedItem;

	private CItemKeyComparer cItemKeyComparer;

	private const string uniqueKeyPrefix = "CollectionController";

	protected CollectionMemento Data => data;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	private void OnValidate()
	{
		if (!collectionWindow)
		{
			collectionWindow = UnityEngine.Object.FindObjectOfType<CollectionWindow>();
		}
		if (!collectionWindow)
		{
			Debug.LogError("Can't find CollectionWindow", this);
		}
	}

	public string UniqueKey()
	{
		return "CollectionController";
	}

	[Inject]
	public void PreInit(GameItemConfigManager gameItemConfigManager)
	{
		this.gameItemConfigManager = gameItemConfigManager;
	}

	public override void Preload()
	{
		base.Preload();
		cItemKeyComparer = new CItemKeyComparer();
		InitializeDefaultData();
		Controller<GameItemController>.Instance.OnItemCreated += AtItemCreated;
		Controller<GameItemController>.Instance.OnItemTakenFromSomethere += AtItemCreated;
		Controller<SelectionController>.Instance.OnSelectionChange += AtSelectionChange;
	}

	public override void Init()
	{
		base.Init();
		collectionWindow.Init();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Controller<GameItemController>.Instance != null)
		{
			Controller<GameItemController>.Instance.OnItemCreated -= AtItemCreated;
			Controller<GameItemController>.Instance.OnItemTakenFromSomethere -= AtItemCreated;
		}
		if (Controller<SelectionController>.Instance != null)
		{
			Controller<SelectionController>.Instance.OnSelectionChange -= AtSelectionChange;
		}
	}

	public Memento SaveState()
	{
		return Data;
	}

	public void LoadState(Memento memento)
	{
		CollectionMemento collectionMemento = (CollectionMemento)memento;
		if (data == null)
		{
			data = collectionMemento;
		}
		else
		{
			UpdateCurrentDataWithSaved(collectionMemento);
		}
	}

	private void UpdateCurrentDataWithSaved(CollectionMemento collectionMemento)
	{
		int num = 0;
		foreach (KeyValuePair<string, CollectionMemento.CGroup> group in collectionMemento.Groups)
		{
			if (!data.Groups.ContainsKey(group.Key))
			{
				data.Groups.Add(group.Key, group.Value);
				num += group.Value.Items.Count;
				continue;
			}
			List<CollectionMemento.CItem> items = data.Groups[group.Key].Items;
			foreach (CollectionMemento.CItem item in group.Value.Items)
			{
				try
				{
					if (item.Key.ID <= items.Count)
					{
						CollectionMemento.CItem cItem = items[item.Key.ID - 1];
						if (cItem != null)
						{
							cItem.Init(item);
							continue;
						}
						items.Add(cItem);
						num++;
					}
				}
				catch (Exception innerException)
				{
					innerException.SendException("Exception when trying to load object \"" + item.Key.ToString() + "\"");
				}
			}
		}
		_ = 0;
	}

	public CollectionMemento InitializeDefaultData()
	{
		if (data == null)
		{
			data = new CollectionMemento(this);
		}
		Dictionary<string, CollectionMemento.CGroup> groups = data.Groups;
		foreach (GIConfig item2 in gameItemConfigManager.Collection)
		{
			GIKey key = item2.Key;
			string collection = key.Collection;
			CollectionMemento.CItem item = new CollectionMemento.CItem(key);
			if (groups.ContainsKey(collection))
			{
				groups[collection].Items.Add(item);
				continue;
			}
			CollectionMemento.CGroup cGroup = new CollectionMemento.CGroup(collection);
			cGroup.Items.Add(item);
			groups.Add(cGroup.CollectionID, cGroup);
		}
		return data;
	}

	public void ShowWindow(GIKey key)
	{
		CollectionMemento.CGroup cGroup = data.Groups[key.Collection];
		GIConfig configOrNull = gameItemConfigManager.GetConfigOrNull(cGroup.Items[key.ID - 1].Key);
		bool needTopPart = configOrNull.HasModule(GIModuleType.ClickSpawn) || configOrNull.HasModule(GIModuleType.AutoSpawn) || configOrNull.HasModule(GIModuleType.Mixer);
		collectionWindow.SetCollection(cGroup, configOrNull, _currentSelectedItem, needTopPart).Show();
	}

	public bool IsLastInCollection(GIKey key)
	{
		return data.Groups[key.Collection].Items.Last().Key == key;
	}

	public GIKey GetMaxOpened(GIKey typeGroupKey)
	{
		CollectionMemento.CGroup cGroup = data.Groups[typeGroupKey.Collection];
		GIKey key = cGroup.Items[0].Key;
		foreach (CollectionMemento.CItem item in cGroup.Items)
		{
			if (item.Opened && item.Key.ID > key.ID)
			{
				key = item.Key;
			}
		}
		return key;
	}

	public IEnumerable<GIKey> GetAllOpened()
	{
		return from x in data.Groups.Values.SelectMany((CollectionMemento.CGroup x) => x.Items)
			where x.Opened
			select x.Key;
	}

	private void AtItemCreated(GameItem item)
	{
		if (!item.IsLocked)
		{
			SetOpenInCollection(item.Config.Key);
		}
	}

	private void AtSelectionChange(GameItem selection)
	{
		_currentSelectedItem = selection;
	}

	public void SetOpenInCollection(GIKey item)
	{
		Data[item].Opened = true;
	}
}
