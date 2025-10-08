using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.Types;
using Merge;
using Merge.Core.Masters;
using StripClub.NewEvent.Data;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

[MementoHolder]
public class GameItemController : Controller<GameItemController>, ISavableState, IMasterController
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		[SerializeField]
		private GeneralData data;

		public bool noNeedMigrate93To94;

		public bool noNeedMigrate173To173a;

		public bool noNeedMigrageOldBalanceToNew;

		public GeneralData Data => data;

		public Memento(GameItemController controller)
			: base(controller)
		{
			noNeedMigrate93To94 = controller.noNeedMigrate93To94 || Data == null;
			noNeedMigrageOldBalanceToNew = controller.noNeedMigrageOldBalanceToNew;
		}
	}

	[SerializeField]
	private FieldMonoMediatorCase fieldMediators;

	private readonly Subject<Unit> _startFieldCreat = new Subject<Unit>();

	private readonly Subject<Unit> _endFieldCreat = new Subject<Unit>();

	public MergeField CurrentField;

	private List<ICreateItemListener> createListeners;

	private List<IRemoveItemListener> removeListeners;

	public ContentType StartType;

	private ReactiveProperty<ContentType> currentContentType = new ReactiveProperty<ContentType>(ContentType.Main);

	private IDisposable disposable;

	private TaskCollect animationController;

	private MergeFieldProvider fieldProvider;

	private GameStarter gameStarter;

	private GameItemFactory gameItemFactory;

	private GameItemDistributor gameItemDistributor;

	private PocketController pocketController;

	private MigrationDeleteMissingItems migrationDeleteMissingItems;

	private EventProvider eventProvider;

	private bool noNeedMigrageOldBalanceToNew;

	private bool noNeedMigrate93To94;

	public IObservable<Unit> StartFieldCreat => (IObservable<Unit>)_startFieldCreat;

	public IObservable<Unit> EndFieldCreat => (IObservable<Unit>)_endFieldCreat;

	public bool Inited { get; private set; }

	public bool IsFull => CurrentField.IsFull;

	public override int PreloadOrder => -1;

	public override int InitOrder => -1;

	public ReadOnlyReactiveProperty<ContentType> CurrentContentType => ReactivePropertyExtensions.ToReadOnlyReactiveProperty<ContentType>((IObservable<ContentType>)currentContentType);

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public event Action<GameItem> OnItemCreated;

	public event Action<GameItem> OnItemRemoved;

	public event Action<GameItem> AfterItemCreated;

	public event Action<GameItem> OnItemTakenFromSomethere;

	public event Action<bool> OnInit;

	[Inject]
	public void Init2(TaskCollect animationController, MergeFieldProvider fieldProvider, GameStarter gameStarter, GameItemFactory gameItemFactory, GameItemDistributor gameItemDistributor, PocketController pocketController, MigrationDeleteMissingItems migrationDeleteMissingItems, EventProvider eventProvider)
	{
		this.gameStarter = gameStarter;
		this.fieldProvider = fieldProvider;
		this.gameItemFactory = gameItemFactory;
		this.gameItemDistributor = gameItemDistributor;
		this.animationController = animationController;
		this.pocketController = pocketController;
		this.migrationDeleteMissingItems = migrationDeleteMissingItems;
		this.eventProvider = eventProvider;
	}

	protected override void OnDestroy()
	{
		disposable?.Dispose();
		base.OnDestroy();
	}

	public override void Preload()
	{
		base.Preload();
		currentContentType.Value = StartType;
		fieldProvider.TryGetData(StartType, out CurrentField);
		fieldProvider.Preload(fieldMediators);
		disposable?.Dispose();
		disposable = ObservableExtensions.Subscribe<MergeField>(Observable.Where<MergeField>(Observable.TakeUntil<MergeField, bool>(Observable.SelectMany<bool, MergeField>(Observable.FirstOrDefault<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), fieldProvider.OnNew), Observable.Take<bool>(Observable.Where<bool>(ReactivePropertyExtensions.SkipLatestValueOnSubscribe<bool>(gameStarter.IsGameActive), (Func<bool, bool>)((bool x) => !x)), 1)), (Func<MergeField, bool>)((MergeField mergeField) => mergeField.Type == ContentType.Event)), (Action<MergeField>)delegate(MergeField mergeField)
		{
			mergeField.SetTransform(fieldMediators.Get(mergeField.Type));
		});
	}

	public void OpenField(ContentType type)
	{
		CurrentField.FieldMediator.Container.SetActive(value: false);
		try
		{
			if (!fieldProvider.TryGetData(type, out var field))
			{
				throw new Exception(GetType().Name + ": doesn't have field of type: " + type).LogException();
			}
			CurrentField = field;
			if (CurrentField.FieldMediator == null)
			{
				Debug.LogError("[CurrentField.FieldMediator] is null");
				return;
			}
		}
		catch (InvalidOperationException innerException)
		{
			throw innerException.SendException(GetType().Name + ": doesn't have field of type: " + type);
		}
		try
		{
			InitField(CurrentField);
		}
		catch (Exception innerException2)
		{
			throw innerException2.SendException("Failed init field of: " + type.ToString() + ". It can be missed items in Items config");
		}
		CurrentField.FieldMediator.Container.SetActive(value: true);
		currentContentType.Value = type;
	}

	public void InitField(MergeField field)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		if (field.Inited)
		{
			return;
		}
		_startFieldCreat?.OnNext(Unit.Default);
		MigrateOldBalanceToNew(field);
		List<GameItem> list = new List<GameItem>();
		InitFieldItems(field, list);
		foreach (GameItem item in list)
		{
			for (int i = 0; i < createListeners.Count; i++)
			{
				createListeners[i].AtItemCreated(item, field);
			}
			this.OnItemCreated?.Invoke(item);
			this.AfterItemCreated?.Invoke(item);
		}
		field.FieldMediator.Container.SetActive(value: false);
		field.Inited = true;
		_endFieldCreat?.OnNext(Unit.Default);
	}

	private void InitFieldItems(MergeField field, List<GameItem> loadedFromSaveItems)
	{
		if ((migrationDeleteMissingItems.IsNeedMainFieldMigration && field.Type == ContentType.Main) || (migrationDeleteMissingItems.IsNeedEventFieldMigration && field.Type == ContentType.Event))
		{
			CreateFieldItemsWithoutBroken(field, loadedFromSaveItems);
			DeleteFieldBrokenItems(field);
			DeletePocketBrokenItems(field);
			migrationDeleteMissingItems.Migrate(field.Type);
		}
		else
		{
			DefaultCreateFieldItems(field, loadedFromSaveItems);
		}
	}

	private List<GIData> GetFieldBrokenItems(MergeField field)
	{
		List<GIData> list = new List<GIData>();
		foreach (GIData gameItem in field.Data.GameItems)
		{
			if (gameItemFactory.IsItemBroken(gameItem))
			{
				list.Add(gameItem);
			}
		}
		return list;
	}

	private void CreateFieldItemsWithoutBroken(MergeField field, List<GameItem> loadedItems)
	{
		foreach (GIData gameItem in field.Data.GameItems)
		{
			if (!gameItemFactory.IsItemBroken(gameItem))
			{
				CreateGameItem(gameItem, field, loadedItems);
			}
		}
	}

	private void DefaultCreateFieldItems(MergeField field, List<GameItem> loadedItems)
	{
		foreach (GIData gameItem in field.Data.GameItems)
		{
			CreateGameItem(gameItem, field, loadedItems);
		}
	}

	private void CreateGameItem(GIData giData, MergeField field, List<GameItem> loadedItems)
	{
		GameItem item = gameItemFactory.Create(giData, field);
		loadedItems.Add(item);
	}

	private List<GIData> GetPocketBrokenItems(ContentType fieldType)
	{
		List<GIData> list = new List<GIData>();
		foreach (GIData item in GetPocketByField(fieldType))
		{
			if (gameItemFactory.IsItemBroken(item))
			{
				list.Add(item);
			}
		}
		return list;
	}

	private Queue<GIData> GetPocketByField(ContentType fieldType)
	{
		if (fieldType != 0)
		{
			return eventProvider.CurrentCalendarProperty.Value.Item2.Data.PocketRepository.Queue;
		}
		return pocketController.PocketItemsQueue;
	}

	private void DeleteFieldBrokenItems(MergeField field)
	{
		List<GIData> fieldBrokenItems = GetFieldBrokenItems(field);
		if (!fieldBrokenItems.Any())
		{
			return;
		}
		foreach (GIData item in fieldBrokenItems)
		{
			GIData gIData = field.Data.GameItems.FirstOrDefault((GIData _item) => _item == item);
			if (gIData != null)
			{
				field.Data.GameItems.Remove(gIData);
			}
		}
	}

	private void DeletePocketBrokenItems(MergeField field)
	{
		List<GIData> pocketBrokenItems = GetPocketBrokenItems(field.Type);
		if (!pocketBrokenItems.Any())
		{
			return;
		}
		Queue<GIData> pocketByField = GetPocketByField(field.Type);
		foreach (GIData item in pocketBrokenItems)
		{
			GIData gIData = pocketByField.FirstOrDefault((GIData _item) => _item == item);
			if (gIData != null)
			{
				if (field.Type == ContentType.Event)
				{
					pocketController.DeleteItemInEventPocket(gIData);
				}
				else
				{
					pocketController.DeleteItemInMainPocket(gIData);
				}
			}
		}
	}

	public override void Init()
	{
		InitField(CurrentField);
		OpenField(StartType);
		StartAnalytic();
	}

	private void StartAnalytic()
	{
		Inited = true;
		this.OnInit?.Invoke(Inited);
	}

	string ISavableState.UniqueKey()
	{
		return "GameItemController";
	}

	GreenT.Data.Memento ISavableState.SaveState()
	{
		fieldProvider.InvokeSaveAllItems();
		return new Memento(this);
	}

	void ISavableState.LoadState(GreenT.Data.Memento memento)
	{
		MigrateSavedDataFrom93To94Version(memento);
		MigrateSavedDataFromOldBalanceToNew(memento);
		if (Inited)
		{
			Init();
		}
	}

	public void PassMergeItem(GIKey key, int count)
	{
		GameItem[] items = FindAvailableItems(key).Take(count).ToArray();
		PassMergeItem(items);
	}

	private void PassMergeItem(IEnumerable<GameItem> items)
	{
		try
		{
			animationController.PlayAnimation(items);
			RemoveItems(items);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	private GameItem CreateItemBase(GIData giData, MergeField field, Action<GameItem> onItemAction, IEnumerable<ICreateItemListener> listeners)
	{
		field = field ?? CurrentField;
		GameItem gameItem = gameItemDistributor.AddItem(giData, field);
		foreach (ICreateItemListener listener in listeners)
		{
			listener.AtItemCreated(gameItem, field);
		}
		onItemAction?.Invoke(gameItem);
		this.AfterItemCreated?.Invoke(gameItem);
		return gameItem;
	}

	public GameItem CreateItem(GIData giData, MergeField field = null)
	{
		return CreateItemBase(giData, field, this.OnItemCreated, createListeners);
	}

	public GameItem TakeItemFromSomethere(GIData giData, MergeField field = null)
	{
		return CreateItemBase(giData, field, this.OnItemTakenFromSomethere, createListeners.Where((ICreateItemListener x) => !(x is MergePointsController)));
	}

	public void RemoveItem(GIKey itemKey, int targetCount)
	{
		List<GameItem> list = FindAvailableItems(itemKey);
		if (list.Count > targetCount)
		{
			list.RemoveRange(targetCount, list.Count - targetCount);
		}
		RemoveItems(list);
	}

	public void RemoveItem(GameItem item, MergeField field = null)
	{
		field = field ?? CurrentField;
		field.RemoveItem(item);
		item.BeginRemove();
		for (int i = 0; i < removeListeners.Count; i++)
		{
			removeListeners[i].AtItemRemoved(item);
		}
		this.OnItemRemoved?.Invoke(item);
		UnityEngine.Object.Destroy(item.gameObject);
	}

	public void RemoveItemsParam(params GameItem[] items)
	{
		RemoveItems(items);
	}

	public void RemoveItems(IEnumerable<GameItem> items)
	{
		foreach (GameItem item in items)
		{
			RemoveItem(item);
		}
	}

	public bool IsPointBusy(Point pnt)
	{
		return CurrentField.Field[pnt] != null;
	}

	public bool TryGetFirstEmptyPoint(out Point pnt)
	{
		int num = 0;
		foreach (GameItem item in CurrentField.Field)
		{
			if (item == null)
			{
				pnt = CurrentField.Field.GetInnerById(num);
				return true;
			}
			num++;
		}
		pnt = default(Point);
		return false;
	}

	public Point GetFirstEmptyPoint()
	{
		TryGetFirstEmptyPoint(out var pnt);
		return pnt;
	}

	public List<Point> GetEmptyTilesDonut(Point centre)
	{
		return GetTilesDonut(centre, (GameItem x) => x == null);
	}

	public List<Point> GetNotEmptyTilesDonut(Point centre)
	{
		return GetTilesDonut(centre, (GameItem x) => x != null);
	}

	public List<Point> GetTilesDonut(Point centre, Func<GameItem, bool> predicate)
	{
		List<Point> list = new List<Point>();
		for (int i = centre.Y - 1; i < centre.Y + 2; i++)
		{
			for (int j = centre.X - 1; j < centre.X + 2; j++)
			{
				if (CurrentField.Field.Contains(j, i) && !centre.Is(j, i) && (predicate == null || predicate(CurrentField.Field[j, i])))
				{
					list.Add(new Point(j, i));
				}
			}
		}
		return list;
	}

	public Point GetNearEmptyPoint(Point point)
	{
		return SpiralSerch(point, (GameItem x) => x == null);
	}

	public Point SpiralSerch(Point start, Func<GameItem, bool> predicate)
	{
		if (SpiralSerchRecurs(start, 0, predicate, out var result))
		{
			return result;
		}
		return start;
	}

	private bool SpiralSerchRecurs(Point position, int size, Func<GameItem, bool> predicate, out Point result)
	{
		position = position.Add(1, 1);
		size += 2;
		bool somethingFound = false;
		Point[] array = new Point[4]
		{
			Point.Down,
			Point.Left,
			Point.Up,
			Point.Right
		};
		for (int i = 0; i < array.Length; i++)
		{
			if (SpiralLine(array[i], out result))
			{
				return true;
			}
		}
		result = default(Point);
		if (!somethingFound)
		{
			return false;
		}
		return SpiralSerchRecurs(position, size, predicate, out result);
		bool SpiralLine(Point direction, out Point innerResult)
		{
			for (int j = 0; j < size; j++)
			{
				position += direction;
				if (CurrentField.Field.Contains(position))
				{
					if (predicate(CurrentField.Field[position]))
					{
						innerResult = position;
						return true;
					}
					somethingFound = true;
				}
			}
			innerResult = default(Point);
			return false;
		}
	}

	public List<GameItem> FindItems(GIKey key, GameItemType itemType)
	{
		return CurrentField.Field.Objects.Where((GameItem _item) => _item.Key.Collection == key.Collection && _item.Config.Type == itemType).ToList();
	}

	public List<GameItem> FindItems(GIKey key)
	{
		return CurrentField.Field.Objects.Where((GameItem _item) => _item.Key == key).ToList();
	}

	public List<GameItem> FindItems(int uniqID)
	{
		return CurrentField.Field.Objects.Where((GameItem _item) => _item.Config.UniqId == uniqID).ToList();
	}

	public List<GameItem> FindAvailableItems(GIKey key)
	{
		return CurrentField.Field.Objects.Where((GameItem _item) => _item.Key == key && IsAvailableItem(_item)).ToList();
	}

	public bool IsAvailableItem(GameItem item)
	{
		if (!item.IsLocked && !item.IsHardLocked)
		{
			return !item.IsBubbled;
		}
		return false;
	}

	public int CalcAvailableItems(GIKey key)
	{
		return CurrentField.Field.Count((GameItem _item) => _item != null && _item.Key == key && IsAvailableItem(_item));
	}

	void IMasterController.LinkControllers(IList<BaseController> controllers)
	{
		createListeners = (from x in controllers.OfType<ICreateItemListener>()
			orderby x.Priority
			select x).ToList();
		removeListeners = (from x in controllers.OfType<IRemoveItemListener>()
			orderby x.Priority
			select x).ToList();
	}

	private void MigrateSavedDataFromOldBalanceToNew(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		noNeedMigrageOldBalanceToNew = memento2.noNeedMigrageOldBalanceToNew;
	}

	private void MigrateOldBalanceToNew(MergeField field)
	{
		if (field.Type == ContentType.Main && !noNeedMigrageOldBalanceToNew)
		{
			ReplaceOldItemsToNew(field);
			noNeedMigrageOldBalanceToNew = true;
		}
	}

	private void ReplaceOldItemsToNew(MergeField field)
	{
		GIKey[] array = new GIKey[0];
		GIKey[] array2 = new GIKey[0];
		Dictionary<GIKey, List<GIData>> dictionary = new Dictionary<GIKey, List<GIData>>();
		foreach (GIData gameItem in field.Data.GameItems)
		{
			if (array.Contains(gameItem.Key))
			{
				if (!dictionary.ContainsKey(gameItem.Key))
				{
					dictionary[gameItem.Key] = new List<GIData>();
				}
				dictionary[gameItem.Key].Add(gameItem);
			}
		}
		GIKey[] array3 = array;
		foreach (GIKey gIKey in array3)
		{
			if (!dictionary.ContainsKey(gIKey))
			{
				continue;
			}
			foreach (GIData item in dictionary[gIKey])
			{
				if (item.HasModule(GIModuleType.Locked))
				{
					int num = Array.IndexOf(array, gIKey);
					item.Key = array2[num];
				}
			}
		}
	}

	private void MigrateSavedDataFrom93To94Version(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		if (!memento2.noNeedMigrate93To94 && memento2.Data != null)
		{
			GeneralData data = CurrentField.Data;
			if (data != null)
			{
				data.ChangeData(memento2.Data.GameItems);
				noNeedMigrate93To94 = true;
			}
		}
	}
}
