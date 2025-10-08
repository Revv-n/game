using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Bonus;
using GreenT.HornyScapes.Bank;
using GreenT.HornyScapes.Bank.BankTabs;
using GreenT.Multiplier;
using ModestTree;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Booster.Effect;

public class SummonBonusService : IInitializable, IDisposable
{
	private readonly ReactiveProperty<(BoosterIncrementBonus bonus, SummonLot summon)> _tupleProperty = new ReactiveProperty<(BoosterIncrementBonus, SummonLot)>();

	private readonly SignalBus _signalBus;

	private readonly LotManager _lotManager;

	private readonly GameStarter _gameStarter;

	private readonly BonusManager _bonusManager;

	private readonly BankTabFinder _bankTabFinder;

	private readonly BoosterStorage _boosterStorage;

	private readonly MultiplierManager _multiplierManager;

	private readonly Subject<int> _onSummonTabUnlocked = new Subject<int>();

	private readonly Subject<SummonLot> _onSummonUnlocked = new Subject<SummonLot>();

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	public SummonBonusService(GameStarter gameStarter, BonusManager bonusManager, MultiplierManager multiplierManager, LotManager lotManager, BoosterStorage boosterStorage, SignalBus signalBus, BankTabFinder bankTabFinder)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		_signalBus = signalBus;
		_bankTabFinder = bankTabFinder;
		_lotManager = lotManager;
		_boosterStorage = boosterStorage;
		_gameStarter = gameStarter;
		_bonusManager = bonusManager;
		_multiplierManager = multiplierManager;
	}

	public void Initialize()
	{
		IObservable<bool> observable = Observable.Take<bool>(Observable.Where<bool>((IObservable<bool>)_gameStarter.IsGameActive, (Func<bool, bool>)((bool status) => status)), 1);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.Where<int>(Observable.Select<double, int>(Observable.SelectMany<bool, double>(observable, (Func<bool, IObservable<double>>)((bool _) => (IObservable<double>)_multiplierManager.SummonMultipliers.Total.Factor)), (Func<double, int>)((double value) => (int)value)), (Func<int, bool>)((int value) => value > 0)), (Action<int>)HandleEffect), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<SummonLot>(Observable.SelectMany<bool, SummonLot>(observable, (Func<bool, IObservable<SummonLot>>)((bool _) => WaitForSummonUnlock())), (Action<SummonLot>)delegate(SummonLot lot)
		{
			_onSummonUnlocked.OnNext(lot);
		}), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(BoosterIncrementBonus, SummonLot)>(Observable.Do<(BoosterIncrementBonus, SummonLot)>(Observable.Do<(BoosterIncrementBonus, SummonLot)>(Observable.Select<SummonLot, (BoosterIncrementBonus, SummonLot)>(Observable.Take<SummonLot>(Observable.Where<SummonLot>(Observable.Where<SummonLot>(Observable.Select<BankTab, SummonLot>(Observable.SelectMany<int[], BankTab>(Observable.Select<(BoosterIncrementBonus, SummonLot), int[]>(Observable.Where<(BoosterIncrementBonus, SummonLot)>((IObservable<(BoosterIncrementBonus, SummonLot)>)_tupleProperty, (Func<(BoosterIncrementBonus, SummonLot), bool>)(((BoosterIncrementBonus bonus, SummonLot summon) tuple) => tuple.bonus != null)), (Func<(BoosterIncrementBonus, SummonLot), int[]>)(((BoosterIncrementBonus bonus, SummonLot summon) tuple) => tuple.bonus.SummonTabID)), (Func<int[], IObservable<BankTab>>)WaitForTargetTabUnlock), (Func<BankTab, SummonLot>)((BankTab tab) => GetSummonByTabID(_tupleProperty.Value.Item1?.SummonType, tab.ID))), (Func<SummonLot, bool>)((SummonLot summon) => summon.ID != _tupleProperty.Value.Item2?.ID)), (Func<SummonLot, bool>)((SummonLot _) => _tupleProperty.Value.Item1 != null)), 1), (Func<SummonLot, (BoosterIncrementBonus, SummonLot)>)((SummonLot newLot) => (bonus: _tupleProperty.Value.Item1, newLot: newLot))), (Action<(BoosterIncrementBonus, SummonLot)>)delegate((BoosterIncrementBonus bonus, SummonLot newLot) tuple)
		{
			tuple.newLot.SetBonus(tuple.bonus);
		}), (Action<(BoosterIncrementBonus, SummonLot)>)delegate((BoosterIncrementBonus bonus, SummonLot newLot) tuple)
		{
			_onSummonTabUnlocked.OnNext(tuple.newLot.TabID);
		}), (Action<(BoosterIncrementBonus, SummonLot)>)Set), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(BoosterIncrementBonus, SummonLot)>(Observable.Where<(BoosterIncrementBonus, SummonLot)>(Observable.SelectMany<(BoosterIncrementBonus, SummonLot), (BoosterIncrementBonus, SummonLot)>(Observable.Where<(BoosterIncrementBonus, SummonLot)>((IObservable<(BoosterIncrementBonus, SummonLot)>)_tupleProperty, (Func<(BoosterIncrementBonus, SummonLot), bool>)(((BoosterIncrementBonus bonus, SummonLot summon) value) => value.bonus != null && value.summon != null)), (Func<(BoosterIncrementBonus, SummonLot), IObservable<(BoosterIncrementBonus, SummonLot)>>)WaitForSummonBuy), (Func<(BoosterIncrementBonus, SummonLot), bool>)delegate((BoosterIncrementBonus bonus, SummonLot summon) value)
		{
			var (boosterIncrementBonus, summonLot) = value;
			var (boosterIncrementBonus2, summonLot2) = _tupleProperty.Value;
			return boosterIncrementBonus == boosterIncrementBonus2 && summonLot == summonLot2;
		}), (Action<(BoosterIncrementBonus, SummonLot)>)delegate((BoosterIncrementBonus bonus, SummonLot summon) tuple)
		{
			tuple.summon.SetBonus(tuple.bonus);
		}), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<BoosterIncrementBonus>(Observable.SelectMany<BoosterIncrementBonus, BoosterIncrementBonus>(Observable.Select<(BoosterIncrementBonus, SummonLot), BoosterIncrementBonus>(Observable.Where<(BoosterIncrementBonus, SummonLot)>((IObservable<(BoosterIncrementBonus, SummonLot)>)_tupleProperty, (Func<(BoosterIncrementBonus, SummonLot), bool>)(((BoosterIncrementBonus bonus, SummonLot summon) value) => value.bonus != null && value.summon != null)), (Func<(BoosterIncrementBonus, SummonLot), BoosterIncrementBonus>)(((BoosterIncrementBonus bonus, SummonLot summon) tuple) => tuple.bonus)), (Func<BoosterIncrementBonus, IObservable<BoosterIncrementBonus>>)OnShouldRestartTimer), (Action<BoosterIncrementBonus>)delegate(BoosterIncrementBonus bonus)
		{
			bonus.StartTimer();
		}), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(BoosterIncrementBonus, SummonLot)>(Observable.Do<(BoosterIncrementBonus, SummonLot)>(Observable.Select<SummonLot, (BoosterIncrementBonus, SummonLot)>(Observable.Take<SummonLot>(Observable.Where<SummonLot>(Observable.Select<BankTab, SummonLot>(Observable.SelectMany<int[], BankTab>(Observable.Select<(BoosterIncrementBonus, SummonLot), int[]>(Observable.SelectMany<(BoosterIncrementBonus, SummonLot), (BoosterIncrementBonus, SummonLot)>(Observable.Where<(BoosterIncrementBonus, SummonLot)>((IObservable<(BoosterIncrementBonus, SummonLot)>)_tupleProperty, (Func<(BoosterIncrementBonus, SummonLot), bool>)(((BoosterIncrementBonus bonus, SummonLot summon) value) => value.bonus != null && value.summon != null)), (Func<(BoosterIncrementBonus, SummonLot), IObservable<(BoosterIncrementBonus, SummonLot)>>)WaitUntilSummonInactive), (Func<(BoosterIncrementBonus, SummonLot), int[]>)(((BoosterIncrementBonus bonus, SummonLot summon) tuple) => tuple.bonus.SummonTabID)), (Func<int[], IObservable<BankTab>>)WaitForTargetTabUnlock), (Func<BankTab, SummonLot>)((BankTab tab) => GetSummonByTabID(_tupleProperty.Value.Item1?.SummonType, tab.ID))), (Func<SummonLot, bool>)((SummonLot _) => _tupleProperty.Value.Item1 != null)), 1), (Func<SummonLot, (BoosterIncrementBonus, SummonLot)>)((SummonLot newLot) => (bonus: _tupleProperty.Value.Item1, newLot: newLot))), (Action<(BoosterIncrementBonus, SummonLot)>)delegate((BoosterIncrementBonus bonus, SummonLot newLot) tuple)
		{
			tuple.newLot.SetBonus(tuple.bonus);
		}), (Action<(BoosterIncrementBonus, SummonLot)>)Set), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<BoosterModel>(Observable.Where<BoosterModel>(_boosterStorage.OnRemove, (Func<BoosterModel, bool>)((BoosterModel booster) => booster.Bonus == _tupleProperty.Value.Item1)), (Action<BoosterModel>)delegate
		{
			Set((bonus: null, newLot: null));
		}), (ICollection<IDisposable>)_compositeDisposable);
	}

	private IObservable<SummonLot> WaitForTargetSummonChange(int tabID)
	{
		return Observable.Take<SummonLot>(Observable.Where<SummonLot>(Observable.Where<SummonLot>(Observable.Where<SummonLot>((IObservable<SummonLot>)_onSummonUnlocked, (Func<SummonLot, bool>)((SummonLot lot) => lot.TabID == tabID)), (Func<SummonLot, bool>)((SummonLot lot) => _bankTabFinder.GetTabs().Any((BankTab tab) => tab.ID == lot.TabID && tab.Lock.IsOpen.Value))), (Func<SummonLot, bool>)((SummonLot lot) => IsSummonAvailable(lot, _tupleProperty.Value.Item1?.SummonType, tabID))), 1);
	}

	private IObservable<BankTab> WaitForTargetTabUnlock(int[] tabIDs)
	{
		BankTab bankTab = _bankTabFinder.GetTabs().FirstOrDefault((BankTab tab) => tabIDs.Any((int item) => tab.ID == item) && tab.Lock.IsOpen.Value);
		if (bankTab != null)
		{
			return Observable.Return<BankTab>(bankTab);
		}
		return Observable.SelectMany<BankTab, BankTab>(Observable.ToObservable<BankTab>(from tab in _bankTabFinder.GetTabs()
			where tabIDs.Any((int item) => item == tab.ID)
			select tab), (Func<BankTab, IObservable<BankTab>>)((BankTab tab) => Observable.Select<bool, BankTab>(Observable.Where<bool>((IObservable<bool>)tab.Lock.IsOpen, (Func<bool, bool>)((bool isOpen) => isOpen)), (Func<bool, BankTab>)((bool _) => tab))));
	}

	private IObservable<BoosterIncrementBonus> OnShouldRestartTimer(BoosterIncrementBonus bonus)
	{
		return Observable.Select<Unit, BoosterIncrementBonus>((IObservable<Unit>)bonus.OnApplied, (Func<Unit, BoosterIncrementBonus>)((Unit _) => bonus));
	}

	private void Set((BoosterIncrementBonus bonus, SummonLot newLot) tuple)
	{
		_tupleProperty.Value.Item2?.ResetBonus();
		_tupleProperty.Value = tuple;
	}

	private IObservable<(BoosterIncrementBonus bonus, SummonLot summon)> WaitUntilSummonInactive((BoosterIncrementBonus bonus, SummonLot summon) tuple)
	{
		return Observable.Select<bool, (BoosterIncrementBonus, SummonLot)>(Observable.Take<bool>(Observable.Where<bool>((IObservable<bool>)tuple.summon.Locker.IsOpen, (Func<bool, bool>)((bool status) => !status)), 1), (Func<bool, (BoosterIncrementBonus, SummonLot)>)((bool _) => tuple));
	}

	private IObservable<(BoosterIncrementBonus bonus, SummonLot summon)> WaitForSummonBuy((BoosterIncrementBonus bonus, SummonLot summon) tuple)
	{
		return Observable.Select<LotBoughtSignal, (BoosterIncrementBonus, SummonLot)>(Observable.Where<LotBoughtSignal>(_signalBus.GetStream<LotBoughtSignal>(), (Func<LotBoughtSignal, bool>)((LotBoughtSignal signal) => signal.Lot.ID == tuple.summon.ID)), (Func<LotBoughtSignal, (BoosterIncrementBonus, SummonLot)>)((LotBoughtSignal _) => tuple));
	}

	private IObservable<SummonLot> WaitForSummonUnlock()
	{
		return Observable.SelectMany<SummonLot, SummonLot>(Observable.ToObservable<SummonLot>(_lotManager.GetLot<SummonLot>()), (Func<SummonLot, IObservable<SummonLot>>)((SummonLot lot) => Observable.Select<bool, SummonLot>(Observable.Where<bool>((IObservable<bool>)lot.Locker.IsOpen, (Func<bool, bool>)((bool status) => status)), (Func<bool, SummonLot>)((bool _) => lot))));
	}

	private void HandleEffect(int rechargeTime)
	{
		MinCompositeMultiplier multiplier = _multiplierManager.SummonMultipliers.Total.GetByValue(rechargeTime);
		if (multiplier == null)
		{
			return;
		}
		BoosterIncrementBonus boosterIncrementBonus = (from item in _bonusManager.Collection
			select item as BoosterIncrementBonus into item
			where item != null
			select item).FirstOrDefault((BoosterIncrementBonus item) => item.Multiplier == multiplier);
		if (boosterIncrementBonus == null || LinqExtensions.IsEmpty<char>((IEnumerable<char>)boosterIncrementBonus.SummonType) || boosterIncrementBonus.SummonTabID == null)
		{
			return;
		}
		if (!_bankTabFinder.TryGetActiveTab(LayoutType.Summon, out var id))
		{
			_tupleProperty.Value = (boosterIncrementBonus, null);
			return;
		}
		SummonLot summonByTabID = GetSummonByTabID(boosterIncrementBonus.SummonType, id);
		if (summonByTabID == null)
		{
			_tupleProperty.Value = (boosterIncrementBonus, null);
			return;
		}
		summonByTabID.SetBonus(boosterIncrementBonus);
		if (boosterIncrementBonus.ApplyTimer.TimeLeft == TimeSpan.Zero)
		{
			boosterIncrementBonus.ApplyTimer.Rewind(TimeSpan.FromSeconds(boosterIncrementBonus.Values));
		}
		_tupleProperty.Value = (boosterIncrementBonus, summonByTabID);
	}

	private SummonLot GetSummonByTabID(string viewType, int tabID)
	{
		return _lotManager.GetLot<SummonLot>().FirstOrDefault((SummonLot item) => IsSummonAvailable(item, viewType, tabID));
	}

	private bool IsSummonAvailable(SummonLot item, string viewType, int tabID)
	{
		if (tabID == item.TabID && item.ViewName == viewType)
		{
			return item.Locker.IsOpen.Value;
		}
		return false;
	}

	public void Dispose()
	{
		CompositeDisposable compositeDisposable = _compositeDisposable;
		if (compositeDisposable != null)
		{
			compositeDisposable.Dispose();
		}
	}
}
