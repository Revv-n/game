using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Meta;
using GreenT.HornyScapes.Meta.Decorations;
using GreenT.HornyScapes.Meta.Navigation;
using GreenT.HornyScapes.Meta.RoomObjects;
using GreenT.HornyScapes.StarShop;
using GreenT.HornyScapes.Tasks.UI;
using GreenT.UI;
using Merge.Meta.RoomObjects;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.UI;

public class StartFlow : MonoBehaviour, IInputBlocker
{
	private IWindowsManager windowsManager;

	private ILootboxOpener lootboxOpener;

	private IWindow emptyWindow;

	private RoomManager house;

	[SerializeField]
	private WindowOpener closeBook;

	[SerializeField]
	private WindowOpener openHousePreparedLootbox;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation afterCloseBookDelay;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation afterOpenMainDelay;

	[SerializeField]
	private WindowID emptyWindowID;

	[SerializeField]
	private WindowGroupID windowGroupToHide;

	private CompositeDisposable disposables = new CompositeDisposable();

	private IDisposable observableEmptyWindow;

	private Subject<IRoomObject<BaseObjectConfig>> onSnappedTo = new Subject<IRoomObject<BaseObjectConfig>>();

	private Subject<IInputBlocker> onUpdate = new Subject<IInputBlocker>();

	private IStarShopItem currentItem;

	private HouseNavigationController navigationController;

	private const float SNAP_DELAY_IN_SECONDS = 1.5f;

	private static readonly TimeSpan snapDelay = TimeSpan.FromSeconds(1.5);

	private Subject<Unit> tryDequeue = new Subject<Unit>();

	private Queue<IRoomObject<BaseObjectConfig>> objectsQueue = new Queue<IRoomObject<BaseObjectConfig>>();

	private GameStarter gameStarter;

	public IObservable<IRoomObject<BaseObjectConfig>> OnSnapedTo => onSnappedTo.AsObservable();

	public IObservable<IInputBlocker> OnUpdate => onUpdate.AsObservable();

	public bool IsLaunched { get; private set; }

	[Inject]
	private void Init(IInputBlockerController blockerController, IWindowsManager windowsManager, RoomManager house, ILootboxOpener lootboxOpener, GameStarter gameStarter)
	{
		blockerController.AddBlocker(this);
		this.windowsManager = windowsManager;
		this.house = house;
		this.lootboxOpener = lootboxOpener;
		this.gameStarter = gameStarter;
	}

	public void Set(HouseNavigationController navigationController)
	{
		this.navigationController = navigationController;
	}

	private void Start()
	{
		observableEmptyWindow = DoOnGetWindow(emptyWindowID, delegate(IWindow _window)
		{
			emptyWindow = _window;
		});
	}

	private IObservable<IWindow> GetWindow(WindowID windowID)
	{
		return gameStarter.IsGameActive.Where((bool x) => x).SelectMany((bool _) => windowsManager.GetWindows(windowID).ToObservable().Merge(windowsManager.OnNew.First((IWindow _window) => _window.WindowID == windowID))
			.First());
	}

	private IDisposable DoOnGetWindow(WindowID id, Action<IWindow> action)
	{
		return GetWindow(id).Subscribe(action, delegate(Exception ex)
		{
			ex.LogException();
		});
	}

	public void Launch(IStarShopItem item)
	{
		currentItem = item;
		IRoomObject<BaseObjectConfig> @object = house.GetObject(currentItem.HouseObjectID);
		objectsQueue.Enqueue(@object);
		IsLaunched = true;
		onUpdate.OnNext(this);
		disposables.Clear();
		emptyWindow?.Close();
		windowsManager.OnCloseWindow += OnCloseTaskBook;
		closeBook.Close();
	}

	public void Launch(Decoration decoration)
	{
		IRoomObject<BaseObjectConfig> @object = house.GetObject(decoration.HouseObjectID);
		objectsQueue.Enqueue(@object);
		IsLaunched = true;
		onUpdate.OnNext(this);
		disposables.Clear();
	}

	private void OnCloseTaskBook(IWindow obj)
	{
		if (obj is TaskBook)
		{
			windowsManager.OnCloseWindow -= OnCloseTaskBook;
			afterCloseBookDelay.Play().OnComplete(OpenHouse);
		}
	}

	private void OpenHouse()
	{
		windowsManager.OnOpenWindow += OnOpenMain;
		openHousePreparedLootbox.OpenOnly();
	}

	private void OnOpenMain(IWindow obj)
	{
		if (obj is EmptyWindow)
		{
			windowsManager.OnOpenWindow -= OnOpenMain;
			afterOpenMainDelay.Play().OnComplete(OnShowLootbox);
		}
	}

	private void OnShowLootbox()
	{
		lootboxOpener.OnOpen.FirstOrDefault((Lootbox _box) => _box.ID == currentItem.LootboxIdReward).Subscribe(delegate
		{
			SnapToObjects();
		}, delegate(Exception ex)
		{
			ex.LogException();
		}).AddTo(disposables);
		lootboxOpener.Open(currentItem.LootboxIdReward, CurrencyAmplitudeAnalytic.SourceType.StarShop);
	}

	public void SetUIVisible(bool visible)
	{
		IEnumerable<IWindow> windows = windowsManager.GetWindows(windowGroupToHide.GetWindows());
		if (visible)
		{
			foreach (IWindow item in windows)
			{
				item.Open();
			}
			return;
		}
		foreach (IWindow item2 in windows)
		{
			item2.Close();
		}
	}

	private void FireOnEnd()
	{
		emptyWindow?.Close();
		SetUIVisible(visible: true);
		navigationController?.Activate(active: true);
		IsLaunched = false;
		onUpdate.OnNext(this);
	}

	public void TryShowFlow()
	{
		if (!IsLaunched && objectsQueue.Count != 0)
		{
			SnapToObjects();
		}
	}

	public void AddObjectToFlow(IGameRoomObject<BaseObjectConfig> roomObject)
	{
		roomObject.SetStatus(EntityStatus.Rewarded);
		roomObject.SetVisible(visible: false);
		objectsQueue.Enqueue(roomObject);
	}

	public void SnapToObjects()
	{
		if (objectsQueue.Count != 0)
		{
			IsLaunched = true;
			onUpdate.OnNext(this);
			navigationController?.Activate(active: false);
			SetUIVisible(visible: false);
			IConnectableObservable<IRoomObject<BaseObjectConfig>> connectableObservable = (from _ in tryDequeue.TakeWhile((Unit _) => objectsQueue.Any())
				select objectsQueue.Dequeue()).Publish();
			connectableObservable.Subscribe(navigationController.SnapToObject).AddTo(disposables);
			connectableObservable.Delay(snapDelay).Subscribe(OnSnappedToObject, delegate(Exception ex)
			{
				ex.LogException();
			}, FireOnEnd).AddTo(disposables);
			onSnappedTo.Subscribe(delegate(IRoomObject<BaseObjectConfig> _object)
			{
				_object.SetVisible(visible: true);
			}).AddTo(disposables);
			connectableObservable.Connect().AddTo(disposables);
			tryDequeue.OnNext(Unit.Default);
		}
	}

	private void OnSnappedToObject(IRoomObject<BaseObjectConfig> obj)
	{
		onSnappedTo.OnNext(obj);
		tryDequeue.OnNext(Unit.Default);
	}

	protected virtual void OnDestroy()
	{
		windowsManager.OnOpenWindow -= OnOpenMain;
		disposables.Dispose();
		observableEmptyWindow.Dispose();
		FreeSubject(onUpdate);
		FreeSubject(onSnappedTo);
		FreeSubject(tryDequeue);
	}

	private void FreeSubject<T>(Subject<T> subject)
	{
		subject.OnCompleted();
		subject.Dispose();
	}
}
