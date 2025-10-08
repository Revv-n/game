using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using StripClub.Model.Quest;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using StripClub.UI;
using StripClub.UI.Shop;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventActivityTabAdministrator : IDisposable
{
	private readonly ActivitiesShopManager _activitiesShopManager;

	private readonly RouletteSummonLotManager _rouletteSummonLotManager;

	private readonly MiniEventTaskManager _taskManager;

	private readonly LotManager _lotManager;

	private readonly SignalBus _signalBus;

	private readonly ISaver _saver;

	private CalendarQueue _calendarQueue;

	private readonly Dictionary<MiniEventActivityTab, CompositeDisposable> _disposables;

	private const int MIN_DOUBLE_OFFERS_AMOUNT = 2;

	public MiniEventActivityTabAdministrator(MiniEventTaskManager taskManager, LotManager lotManager, ActivitiesShopManager activitiesShopManager, RouletteSummonLotManager rouletteSummonLotManager, SignalBus signalBus, ISaver saver, CalendarQueue calendarQueue)
	{
		_taskManager = taskManager;
		_lotManager = lotManager;
		_activitiesShopManager = activitiesShopManager;
		_rouletteSummonLotManager = rouletteSummonLotManager;
		_signalBus = signalBus;
		_saver = saver;
		_calendarQueue = calendarQueue;
		_disposables = new Dictionary<MiniEventActivityTab, CompositeDisposable>();
	}

	public void Dispose()
	{
		foreach (CompositeDisposable value in _disposables.Values)
		{
			if (value != null)
			{
				value.Clear();
			}
			if (value != null)
			{
				value.Dispose();
			}
		}
	}

	public void AdministrateActivityTab(MiniEventActivityTab miniEventActivityTab)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		CompositeDisposable val = new CompositeDisposable();
		_disposables.Add(miniEventActivityTab, val);
		switch (miniEventActivityTab.TabType)
		{
		case TabType.Task:
			HandleTasksTab(miniEventActivityTab, val);
			break;
		case TabType.Shop:
			HandleShopTab(miniEventActivityTab, val);
			break;
		case TabType.Rating:
			HandleRatingTab(miniEventActivityTab);
			break;
		}
	}

	public void DisposeTabs(IEnumerable<MiniEventActivityTab> tabs)
	{
		foreach (MiniEventActivityTab tab in tabs)
		{
			if (_disposables.ContainsKey(tab))
			{
				CompositeDisposable obj = _disposables[tab];
				if (obj != null)
				{
					obj.Dispose();
				}
				CompositeDisposable obj2 = _disposables[tab];
				if (obj2 != null)
				{
					obj2.Clear();
				}
				_disposables.Remove(tab);
				tab.IsAnyActionAvailable.Value = false;
				tab.IsAnyContentAvailable.Value = false;
			}
			ActivityShopMapper activityShopMapper = _activitiesShopManager.Collection.FirstOrDefault((ActivityShopMapper activityShop) => activityShop.bank_tab_id == tab.DataIdentificator[0]);
			if (activityShopMapper == null)
			{
				break;
			}
			IEnumerable<Lot> enumerable = _lotManager.Collection.Where((Lot lot) => lot.TabID == activityShopMapper.bank_tab_id);
			if (enumerable == null)
			{
				break;
			}
			foreach (Lot item in enumerable)
			{
				if (item is BundleLot bundleLot)
				{
					bundleLot.ForceReset();
				}
				_saver.Delete(item);
			}
		}
	}

	private void HandleTasksTab(MiniEventActivityTab miniEventActivityTab, CompositeDisposable disposable)
	{
		IEnumerable<MiniEventTask> tasks = _taskManager.Tasks.Where((MiniEventTask task) => task.Identificator == miniEventActivityTab.DataIdentificator);
		foreach (MiniEventTask item in tasks)
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<StateType>(ObserveExtensions.ObserveEveryValueChanged<MiniEventTask, StateType>(item, (Func<MiniEventTask, StateType>)((MiniEventTask state) => state.State), (FrameCountType)0, false), (Action<StateType>)delegate
			{
				miniEventActivityTab.IsAnyActionAvailable.Value = tasks.Any((MiniEventTask task) => task.State == StateType.Complete);
				miniEventActivityTab.IsAnyContentAvailable.Value = tasks.Any((MiniEventTask task) => task.State == StateType.Complete || task.State == StateType.Active);
			}), (ICollection<IDisposable>)disposable);
		}
	}

	private void HandleShopTab(MiniEventActivityTab miniEventActivityTab, CompositeDisposable disposable)
	{
		ActivityShopMapper activityShopMapper = _activitiesShopManager.Collection.FirstOrDefault((ActivityShopMapper activityShop) => activityShop.bank_tab_id == miniEventActivityTab.DataIdentificator[0]);
		if (activityShopMapper.tab_type == ShopTabType.Roulette)
		{
			HandleRouletteActivityShop(activityShopMapper, miniEventActivityTab, disposable);
		}
		else
		{
			HandleDefaultActivityShop(activityShopMapper, miniEventActivityTab, disposable);
		}
	}

	private void HandleRatingTab(MiniEventActivityTab miniEventActivityTab)
	{
		miniEventActivityTab.IsAnyContentAvailable.Value = true;
	}

	private void HandleDefaultActivityShop(ActivityShopMapper activityShopMapper, MiniEventActivityTab miniEventActivityTab, CompositeDisposable disposable)
	{
		IDisposable disposable2 = null;
		IEnumerable<Lot> lots = null;
		lots = _lotManager.Collection.Where((Lot lot) => lot.TabID == activityShopMapper.bank_tab_id);
		if (lots == null)
		{
			return;
		}
		IObservable<Unit> observable = Observable.AsUnitObservable<bool>(Observable.Merge<bool>(Observable.SelectMany<Lot, bool>(Observable.ToObservable<Lot>(lots), (Func<Lot, IObservable<bool>>)((Lot _lot) => Observable.Where<bool>((IObservable<bool>)_lot.Locker.IsOpen, (Func<bool, bool>)((bool _isOpen) => _isOpen)))), Array.Empty<IObservable<bool>>()));
		disposable2 = ObservableExtensions.Subscribe<Unit>(Observable.Merge<Unit>(Observable.Merge<Unit>(Observable.AsUnitObservable<ViewUpdateSignal>(Observable.Where<ViewUpdateSignal>(_signalBus.GetStream<ViewUpdateSignal>(), (Func<ViewUpdateSignal, bool>)((ViewUpdateSignal _signal) => _signal.View is IView<Lot>))), new IObservable<Unit>[1] { Observable.AsUnitObservable<LotBoughtSignal>(_signalBus.GetStream<LotBoughtSignal>()) }), new IObservable<Unit>[1] { observable }), (Action<Unit>)delegate
		{
			if (activityShopMapper.tab_type == ShopTabType.DoubleOffer)
			{
				bool flag = lots.Where((Lot lot) => lot.IsAvailable()).Count() >= 2;
				miniEventActivityTab.IsAnyActionAvailable.Value = lots.Any((Lot lot) => lot.IsAvailable() && !lot.IsViewed) && flag;
				miniEventActivityTab.IsAnyContentAvailable.Value = lots.Any((Lot lot) => lot.IsAvailable()) && flag;
			}
			else
			{
				miniEventActivityTab.IsAnyActionAvailable.Value = lots.Any((Lot lot) => lot.IsAvailable() && !lot.IsViewed);
				miniEventActivityTab.IsAnyContentAvailable.Value = lots.Any((Lot lot) => lot.IsAvailable());
			}
		});
		disposable.Add(disposable2);
	}

	private void HandleRouletteActivityShop(ActivityShopMapper activityShopMapper, MiniEventActivityTab miniEventActivityTab, CompositeDisposable disposable)
	{
		IDisposable disposable2 = null;
		IEnumerable<RouletteSummonLot> rouletteLots = null;
		rouletteLots = _rouletteSummonLotManager.Collection.Where((RouletteSummonLot lot) => lot.ID == activityShopMapper.bank_tab_id);
		if (rouletteLots == null)
		{
			return;
		}
		IObservable<Unit> observable = Observable.AsUnitObservable<bool>(Observable.Merge<bool>(Observable.SelectMany<RouletteSummonLot, bool>(Observable.ToObservable<RouletteSummonLot>(rouletteLots), (Func<RouletteSummonLot, IObservable<bool>>)((RouletteSummonLot _lot) => Observable.Where<bool>((IObservable<bool>)_lot.Locker.IsOpen, (Func<bool, bool>)((bool _isOpen) => _isOpen)))), Array.Empty<IObservable<bool>>()));
		disposable2 = ObservableExtensions.Subscribe<Unit>(Observable.Merge<Unit>(Observable.Merge<Unit>(Observable.AsUnitObservable<ViewUpdateSignal>(Observable.Where<ViewUpdateSignal>(_signalBus.GetStream<ViewUpdateSignal>(), (Func<ViewUpdateSignal, bool>)((ViewUpdateSignal _signal) => _signal.View is IView<RouletteLot>))), new IObservable<Unit>[1] { Observable.AsUnitObservable<LotBoughtSignal>(_signalBus.GetStream<LotBoughtSignal>()) }), new IObservable<Unit>[1] { observable }), (Action<Unit>)delegate
		{
			miniEventActivityTab.IsAnyActionAvailable.Value = rouletteLots.Any((RouletteSummonLot lot) => lot.Locker.IsOpen.Value && !lot.IsViewed);
			miniEventActivityTab.IsAnyContentAvailable.Value = rouletteLots.Any((RouletteSummonLot lot) => lot.Locker.IsOpen.Value);
		});
		disposable.Add(disposable2);
	}
}
