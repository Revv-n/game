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
			value?.Clear();
			value?.Dispose();
		}
	}

	public void AdministrateActivityTab(MiniEventActivityTab miniEventActivityTab)
	{
		CompositeDisposable compositeDisposable = new CompositeDisposable();
		_disposables.Add(miniEventActivityTab, compositeDisposable);
		switch (miniEventActivityTab.TabType)
		{
		case TabType.Task:
			HandleTasksTab(miniEventActivityTab, compositeDisposable);
			break;
		case TabType.Shop:
			HandleShopTab(miniEventActivityTab, compositeDisposable);
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
				_disposables[tab]?.Dispose();
				_disposables[tab]?.Clear();
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
			item.ObserveEveryValueChanged((MiniEventTask state) => state.State).Subscribe(delegate
			{
				miniEventActivityTab.IsAnyActionAvailable.Value = tasks.Any((MiniEventTask task) => task.State == StateType.Complete);
				miniEventActivityTab.IsAnyContentAvailable.Value = tasks.Any((MiniEventTask task) => task.State == StateType.Complete || task.State == StateType.Active);
			}).AddTo(disposable);
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
		IObservable<Unit> observable = lots.ToObservable().SelectMany((Lot _lot) => _lot.Locker.IsOpen.Where((bool _isOpen) => _isOpen)).Merge()
			.AsUnitObservable();
		disposable2 = (from _signal in _signalBus.GetStream<ViewUpdateSignal>()
			where _signal.View is IView<Lot>
			select _signal).AsUnitObservable().Merge(_signalBus.GetStream<LotBoughtSignal>().AsUnitObservable()).Merge(observable)
			.Subscribe(delegate
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
		IObservable<Unit> observable = rouletteLots.ToObservable().SelectMany((RouletteSummonLot _lot) => _lot.Locker.IsOpen.Where((bool _isOpen) => _isOpen)).Merge()
			.AsUnitObservable();
		disposable2 = (from _signal in _signalBus.GetStream<ViewUpdateSignal>()
			where _signal.View is IView<RouletteLot>
			select _signal).AsUnitObservable().Merge(_signalBus.GetStream<LotBoughtSignal>().AsUnitObservable()).Merge(observable)
			.Subscribe(delegate
			{
				miniEventActivityTab.IsAnyActionAvailable.Value = rouletteLots.Any((RouletteSummonLot lot) => lot.Locker.IsOpen.Value && !lot.IsViewed);
				miniEventActivityTab.IsAnyContentAvailable.Value = rouletteLots.Any((RouletteSummonLot lot) => lot.Locker.IsOpen.Value);
			});
		disposable.Add(disposable2);
	}
}
