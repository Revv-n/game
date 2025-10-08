using System;
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
		IObservable<bool> source = _gameStarter.IsGameActive.Where((bool status) => status).Take(1);
		(from value in source.SelectMany((bool _) => _multiplierManager.SummonMultipliers.Total.Factor)
			select (int)value into value
			where value > 0
			select value).Subscribe(HandleEffect).AddTo(_compositeDisposable);
		source.SelectMany((bool _) => WaitForSummonUnlock()).Subscribe(delegate(SummonLot lot)
		{
			_onSummonUnlocked.OnNext(lot);
		}).AddTo(_compositeDisposable);
		(from newLot in (from tab in (from tuple in _tupleProperty
					where tuple.bonus != null
					select tuple.bonus.SummonTabID).SelectMany((Func<int[], IObservable<BankTab>>)WaitForTargetTabUnlock)
				select GetSummonByTabID(_tupleProperty.Value.bonus?.SummonType, tab.ID) into summon
				where summon.ID != _tupleProperty.Value.summon?.ID
				select summon into _
				where _tupleProperty.Value.bonus != null
				select _).Take(1)
			select (bonus: _tupleProperty.Value.bonus, newLot: newLot)).Do(delegate((BoosterIncrementBonus bonus, SummonLot newLot) tuple)
		{
			tuple.newLot.SetBonus(tuple.bonus);
		}).Do(delegate((BoosterIncrementBonus bonus, SummonLot newLot) tuple)
		{
			_onSummonTabUnlocked.OnNext(tuple.newLot.TabID);
		}).Subscribe(Set)
			.AddTo(_compositeDisposable);
		_tupleProperty.Where(((BoosterIncrementBonus bonus, SummonLot summon) value) => value.bonus != null && value.summon != null).SelectMany<(BoosterIncrementBonus, SummonLot), (BoosterIncrementBonus, SummonLot)>((Func<(BoosterIncrementBonus, SummonLot), IObservable<(BoosterIncrementBonus, SummonLot)>>)WaitForSummonBuy).Where<(BoosterIncrementBonus, SummonLot)>(delegate((BoosterIncrementBonus bonus, SummonLot summon) value)
		{
			var (boosterIncrementBonus, summonLot) = value;
			var (boosterIncrementBonus2, summonLot2) = _tupleProperty.Value;
			return boosterIncrementBonus == boosterIncrementBonus2 && summonLot == summonLot2;
		})
			.Subscribe<(BoosterIncrementBonus, SummonLot)>(delegate((BoosterIncrementBonus bonus, SummonLot summon) tuple)
			{
				tuple.summon.SetBonus(tuple.bonus);
			})
			.AddTo(_compositeDisposable);
		(from value in _tupleProperty
			where value.bonus != null && value.summon != null
			select value into tuple
			select tuple.bonus).SelectMany((Func<BoosterIncrementBonus, IObservable<BoosterIncrementBonus>>)OnShouldRestartTimer).Subscribe(delegate(BoosterIncrementBonus bonus)
		{
			bonus.StartTimer();
		}).AddTo(_compositeDisposable);
		(from newLot in (from tab in (from tuple in _tupleProperty.Where(((BoosterIncrementBonus bonus, SummonLot summon) value) => value.bonus != null && value.summon != null).SelectMany<(BoosterIncrementBonus, SummonLot), (BoosterIncrementBonus, SummonLot)>((Func<(BoosterIncrementBonus, SummonLot), IObservable<(BoosterIncrementBonus, SummonLot)>>)WaitUntilSummonInactive)
					select tuple.bonus.SummonTabID).SelectMany((Func<int[], IObservable<BankTab>>)WaitForTargetTabUnlock)
				select GetSummonByTabID(_tupleProperty.Value.bonus?.SummonType, tab.ID) into _
				where _tupleProperty.Value.bonus != null
				select _).Take(1)
			select (bonus: _tupleProperty.Value.bonus, newLot: newLot)).Do(delegate((BoosterIncrementBonus bonus, SummonLot newLot) tuple)
		{
			tuple.newLot.SetBonus(tuple.bonus);
		}).Subscribe(Set).AddTo(_compositeDisposable);
		_boosterStorage.OnRemove.Where((BoosterModel booster) => booster.Bonus == _tupleProperty.Value.bonus).Subscribe(delegate
		{
			Set((bonus: null, newLot: null));
		}).AddTo(_compositeDisposable);
	}

	private IObservable<SummonLot> WaitForTargetSummonChange(int tabID)
	{
		return (from lot in _onSummonUnlocked
			where lot.TabID == tabID
			where _bankTabFinder.GetTabs().Any((BankTab tab) => tab.ID == lot.TabID && tab.Lock.IsOpen.Value)
			where IsSummonAvailable(lot, _tupleProperty.Value.bonus?.SummonType, tabID)
			select lot).Take(1);
	}

	private IObservable<BankTab> WaitForTargetTabUnlock(int[] tabIDs)
	{
		BankTab bankTab = _bankTabFinder.GetTabs().FirstOrDefault((BankTab tab) => tabIDs.Any((int item) => tab.ID == item) && tab.Lock.IsOpen.Value);
		if (bankTab != null)
		{
			return Observable.Return(bankTab);
		}
		return (from tab in _bankTabFinder.GetTabs()
			where tabIDs.Any((int item) => item == tab.ID)
			select tab).ToObservable().SelectMany((BankTab tab) => from isOpen in tab.Lock.IsOpen
			where isOpen
			select isOpen into _
			select tab);
	}

	private IObservable<BoosterIncrementBonus> OnShouldRestartTimer(BoosterIncrementBonus bonus)
	{
		return bonus.OnApplied.Select((Unit _) => bonus);
	}

	private void Set((BoosterIncrementBonus bonus, SummonLot newLot) tuple)
	{
		_tupleProperty.Value.summon?.ResetBonus();
		_tupleProperty.Value = tuple;
	}

	private IObservable<(BoosterIncrementBonus bonus, SummonLot summon)> WaitUntilSummonInactive((BoosterIncrementBonus bonus, SummonLot summon) tuple)
	{
		return from _ in tuple.summon.Locker.IsOpen.Where((bool status) => !status).Take(1)
			select tuple;
	}

	private IObservable<(BoosterIncrementBonus bonus, SummonLot summon)> WaitForSummonBuy((BoosterIncrementBonus bonus, SummonLot summon) tuple)
	{
		return from signal in _signalBus.GetStream<LotBoughtSignal>()
			where signal.Lot.ID == tuple.summon.ID
			select signal into _
			select tuple;
	}

	private IObservable<SummonLot> WaitForSummonUnlock()
	{
		return _lotManager.GetLot<SummonLot>().ToObservable().SelectMany((SummonLot lot) => from status in lot.Locker.IsOpen
			where status
			select status into _
			select lot);
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
		if (boosterIncrementBonus == null || boosterIncrementBonus.SummonType.IsEmpty() || boosterIncrementBonus.SummonTabID == null)
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
		_compositeDisposable?.Dispose();
	}
}
