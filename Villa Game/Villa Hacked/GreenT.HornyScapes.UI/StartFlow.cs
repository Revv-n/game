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
	private Animation afterCloseBookDelay;

	[SerializeField]
	private Animation afterOpenMainDelay;

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

	public IObservable<IRoomObject<BaseObjectConfig>> OnSnapedTo => Observable.AsObservable<IRoomObject<BaseObjectConfig>>((IObservable<IRoomObject<BaseObjectConfig>>)onSnappedTo);

	public IObservable<IInputBlocker> OnUpdate => Observable.AsObservable<IInputBlocker>((IObservable<IInputBlocker>)onUpdate);

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
		return Observable.SelectMany<bool, IWindow>(Observable.Where<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), (Func<bool, IObservable<IWindow>>)((bool _) => Observable.First<IWindow>(Observable.Merge<IWindow>(Observable.ToObservable<IWindow>(windowsManager.GetWindows(windowID)), new IObservable<IWindow>[1] { Observable.First<IWindow>(windowsManager.OnNew, (Func<IWindow, bool>)((IWindow _window) => _window.WindowID == windowID)) }))));
	}

	private IDisposable DoOnGetWindow(WindowID id, Action<IWindow> action)
	{
		return ObservableExtensions.Subscribe<IWindow>(GetWindow(id), action, (Action<Exception>)delegate(Exception ex)
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
		onUpdate.OnNext((IInputBlocker)this);
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
		onUpdate.OnNext((IInputBlocker)this);
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Lootbox>(Observable.FirstOrDefault<Lootbox>(lootboxOpener.OnOpen, (Func<Lootbox, bool>)((Lootbox _box) => _box.ID == currentItem.LootboxIdReward)), (Action<Lootbox>)delegate
		{
			SnapToObjects();
		}, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		}), (ICollection<IDisposable>)disposables);
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
		onUpdate.OnNext((IInputBlocker)this);
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
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		if (objectsQueue.Count != 0)
		{
			IsLaunched = true;
			onUpdate.OnNext((IInputBlocker)this);
			navigationController?.Activate(active: false);
			SetUIVisible(visible: false);
			IConnectableObservable<IRoomObject<BaseObjectConfig>> obj = Observable.Publish<IRoomObject<BaseObjectConfig>>(Observable.Select<Unit, IRoomObject<BaseObjectConfig>>(Observable.TakeWhile<Unit>((IObservable<Unit>)tryDequeue, (Func<Unit, bool>)((Unit _) => objectsQueue.Any())), (Func<Unit, IRoomObject<BaseObjectConfig>>)((Unit _) => objectsQueue.Dequeue())));
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IRoomObject<BaseObjectConfig>>((IObservable<IRoomObject<BaseObjectConfig>>)obj, (Action<IRoomObject<BaseObjectConfig>>)navigationController.SnapToObject), (ICollection<IDisposable>)disposables);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IRoomObject<BaseObjectConfig>>(Observable.Delay<IRoomObject<BaseObjectConfig>>((IObservable<IRoomObject<BaseObjectConfig>>)obj, snapDelay), (Action<IRoomObject<BaseObjectConfig>>)OnSnappedToObject, (Action<Exception>)delegate(Exception ex)
			{
				ex.LogException();
			}, (Action)FireOnEnd), (ICollection<IDisposable>)disposables);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IRoomObject<BaseObjectConfig>>((IObservable<IRoomObject<BaseObjectConfig>>)onSnappedTo, (Action<IRoomObject<BaseObjectConfig>>)delegate(IRoomObject<BaseObjectConfig> _object)
			{
				_object.SetVisible(visible: true);
			}), (ICollection<IDisposable>)disposables);
			DisposableExtensions.AddTo<IDisposable>(obj.Connect(), (ICollection<IDisposable>)disposables);
			tryDequeue.OnNext(Unit.Default);
		}
	}

	private void OnSnappedToObject(IRoomObject<BaseObjectConfig> obj)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		onSnappedTo.OnNext(obj);
		tryDequeue.OnNext(Unit.Default);
	}

	protected virtual void OnDestroy()
	{
		windowsManager.OnOpenWindow -= OnOpenMain;
		disposables.Dispose();
		observableEmptyWindow.Dispose();
		FreeSubject<IInputBlocker>(onUpdate);
		FreeSubject<IRoomObject<BaseObjectConfig>>(onSnappedTo);
		FreeSubject<Unit>(tryDequeue);
	}

	private void FreeSubject<T>(Subject<T> subject)
	{
		subject.OnCompleted();
		subject.Dispose();
	}
}
