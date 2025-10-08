using System;
using Merge.Meta.RoomObjects;
using StripClub.Model.Shop;
using UniRx;

namespace GreenT.HornyScapes.StarShop.SubSystems;

public class StarShopSubscriptions : IDisposable
{
	private CompositeDisposable disposables = new CompositeDisposable();

	private ICurrencyProcessor currencyProcessor;

	private StarShopManager manager;

	public StarShopSubscriptions(StarShopManager manager, ICurrencyProcessor currencyProcessor)
	{
		this.currencyProcessor = currencyProcessor;
		this.manager = manager;
	}

	public void Activate(bool isGameActive)
	{
		disposables.Clear();
		if (isGameActive)
		{
			IConnectableObservable<StarShopItem> connectableObservable = (from _item in manager.Collection.ToObservable().Merge(manager.OnNew)
				where _item.State != EntityStatus.Rewarded
				select _item).Publish();
			IConnectableObservable<StarShopItem> connectableObservable2 = ((IObservable<StarShopItem>)connectableObservable).SelectMany((Func<StarShopItem, IObservable<StarShopItem>>)EmitOnLockerSwitched).Publish();
			connectableObservable2.Subscribe(DetermineState).AddTo(disposables);
			connectableObservable.Where(IsItemActive).SelectMany((Func<StarShopItem, IObservable<StarShopItem>>)EmitOnBalanceChanged).Subscribe(DetermineState)
				.AddTo(disposables);
			connectableObservable2.Where((StarShopItem _item) => _item.Lock.IsOpen.Value).SelectMany((Func<StarShopItem, IObservable<StarShopItem>>)EmitOnBalanceChangedWhileLockerOpen).Subscribe(DetermineState)
				.AddTo(disposables);
			connectableObservable2.Connect().AddTo(disposables);
			connectableObservable.Connect().AddTo(disposables);
		}
	}

	private void DetermineState(StarShopItem item)
	{
		EntityStatus state = EntityStatus.Blocked;
		if (item.Lock.IsOpen.Value)
		{
			state = (currencyProcessor.IsEnough(item.Cost) ? EntityStatus.Complete : EntityStatus.InProgress);
		}
		item.SetState(state);
	}

	private IObservable<StarShopItem> EmitOnLockerSwitched(StarShopItem item)
	{
		return from _ in item.Lock.IsOpen.Where(NeedToSwitch).TakeWhile((bool _) => item.State != EntityStatus.Rewarded)
			select item;
		bool ItemHaveToBeBlocked(bool isOpen)
		{
			if (!isOpen)
			{
				return IsItemActive(item);
			}
			return false;
		}
		bool ItemHaveToBeUnblocked(bool isOpen)
		{
			if (isOpen)
			{
				return item.State == EntityStatus.Blocked;
			}
			return false;
		}
		bool NeedToSwitch(bool isOpen)
		{
			if (!ItemHaveToBeUnblocked(isOpen))
			{
				return ItemHaveToBeBlocked(isOpen);
			}
			return true;
		}
	}

	private IObservable<StarShopItem> EmitOnBalanceChanged(StarShopItem item)
	{
		return (from _ in GetPlayerCurrency(item)
			select item).TakeWhile(IsItemActive);
	}

	private IReadOnlyReactiveProperty<int> GetPlayerCurrency(StarShopItem item)
	{
		return currencyProcessor.GetCountReactiveProperty(item.Cost.Currency);
	}

	private IObservable<StarShopItem> EmitOnBalanceChangedWhileLockerOpen(StarShopItem item)
	{
		return (from _ in GetPlayerCurrency(item).Skip(1)
			select item).TakeWhile((StarShopItem _item) => _item.State != EntityStatus.Rewarded);
	}

	public void Dispose()
	{
		disposables?.Dispose();
	}

	private static bool IsItemActive(StarShopItem item)
	{
		if (item.State != EntityStatus.InProgress)
		{
			return item.State == EntityStatus.Complete;
		}
		return true;
	}
}
