using System;
using System.Collections.Generic;
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		this.currencyProcessor = currencyProcessor;
		this.manager = manager;
	}

	public void Activate(bool isGameActive)
	{
		disposables.Clear();
		if (isGameActive)
		{
			IConnectableObservable<StarShopItem> obj = Observable.Publish<StarShopItem>(Observable.Where<StarShopItem>(Observable.Merge<StarShopItem>(Observable.ToObservable<StarShopItem>(manager.Collection), new IObservable<StarShopItem>[1] { manager.OnNew }), (Func<StarShopItem, bool>)((StarShopItem _item) => _item.State != EntityStatus.Rewarded)));
			IConnectableObservable<StarShopItem> val = Observable.Publish<StarShopItem>(Observable.SelectMany<StarShopItem, StarShopItem>((IObservable<StarShopItem>)obj, (Func<StarShopItem, IObservable<StarShopItem>>)EmitOnLockerSwitched));
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<StarShopItem>((IObservable<StarShopItem>)val, (Action<StarShopItem>)DetermineState), (ICollection<IDisposable>)disposables);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<StarShopItem>(Observable.SelectMany<StarShopItem, StarShopItem>(Observable.Where<StarShopItem>((IObservable<StarShopItem>)obj, (Func<StarShopItem, bool>)IsItemActive), (Func<StarShopItem, IObservable<StarShopItem>>)EmitOnBalanceChanged), (Action<StarShopItem>)DetermineState), (ICollection<IDisposable>)disposables);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<StarShopItem>(Observable.SelectMany<StarShopItem, StarShopItem>(Observable.Where<StarShopItem>((IObservable<StarShopItem>)val, (Func<StarShopItem, bool>)((StarShopItem _item) => _item.Lock.IsOpen.Value)), (Func<StarShopItem, IObservable<StarShopItem>>)EmitOnBalanceChangedWhileLockerOpen), (Action<StarShopItem>)DetermineState), (ICollection<IDisposable>)disposables);
			DisposableExtensions.AddTo<IDisposable>(val.Connect(), (ICollection<IDisposable>)disposables);
			DisposableExtensions.AddTo<IDisposable>(obj.Connect(), (ICollection<IDisposable>)disposables);
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
		return Observable.Select<bool, StarShopItem>(Observable.TakeWhile<bool>(Observable.Where<bool>((IObservable<bool>)item.Lock.IsOpen, (Func<bool, bool>)NeedToSwitch), (Func<bool, bool>)((bool _) => item.State != EntityStatus.Rewarded)), (Func<bool, StarShopItem>)((bool _) => item));
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
		return Observable.TakeWhile<StarShopItem>(Observable.Select<int, StarShopItem>((IObservable<int>)GetPlayerCurrency(item), (Func<int, StarShopItem>)((int _) => item)), (Func<StarShopItem, bool>)IsItemActive);
	}

	private IReadOnlyReactiveProperty<int> GetPlayerCurrency(StarShopItem item)
	{
		return currencyProcessor.GetCountReactiveProperty(item.Cost.Currency);
	}

	private IObservable<StarShopItem> EmitOnBalanceChangedWhileLockerOpen(StarShopItem item)
	{
		return Observable.TakeWhile<StarShopItem>(Observable.Select<int, StarShopItem>(Observable.Skip<int>((IObservable<int>)GetPlayerCurrency(item), 1), (Func<int, StarShopItem>)((int _) => item)), (Func<StarShopItem, bool>)((StarShopItem _item) => _item.State != EntityStatus.Rewarded));
	}

	public void Dispose()
	{
		CompositeDisposable obj = disposables;
		if (obj != null)
		{
			obj.Dispose();
		}
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
