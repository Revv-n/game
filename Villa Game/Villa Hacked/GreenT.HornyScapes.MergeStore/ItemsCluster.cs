using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using Merge;
using UniRx;

namespace GreenT.HornyScapes.MergeStore;

public class ItemsCluster
{
	private readonly Subject<ItemBuyRequest> _onPurchasedRequest = new Subject<ItemBuyRequest>();

	private readonly Subject<GIKey> _onUpdate = new Subject<GIKey>();

	private readonly Dictionary<GIKey, IDisposable> _subscriptions = new Dictionary<GIKey, IDisposable>();

	private readonly Dictionary<GIKey, IItem> _assortment = new Dictionary<GIKey, IItem>();

	private readonly Dictionary<StoreSection, Dictionary<string, HashSet<int>>> _currentAssortment = new Dictionary<StoreSection, Dictionary<string, HashSet<int>>>();

	public IObservable<ItemBuyRequest> OnPurchasedRequest => (IObservable<ItemBuyRequest>)_onPurchasedRequest;

	public IObservable<GIKey> OnUpdate => (IObservable<GIKey>)_onUpdate;

	public bool TryGet(GIKey key, out IItem result)
	{
		return _assortment.TryGetValue(key, out result);
	}

	public bool Have(ContentType contentType, GIKey key)
	{
		HashSet<int> value;
		return _currentAssortment.Any((System.Collections.Generic.KeyValuePair<StoreSection, Dictionary<string, HashSet<int>>> x) => x.Key.ContentType.Equals(contentType) && x.Value.TryGetValue(key.Collection, out value) && value.Contains(key.ID));
	}

	public bool HaveLessOrEqual(ContentType contentType, GIKey key)
	{
		if (_currentAssortment.All((System.Collections.Generic.KeyValuePair<StoreSection, Dictionary<string, HashSet<int>>> x) => x.Key.ContentType != contentType))
		{
			return false;
		}
		Dictionary<string, HashSet<int>> dictionary = _currentAssortment.Select((System.Collections.Generic.KeyValuePair<StoreSection, Dictionary<string, HashSet<int>>> x) => x.Value).FirstOrDefault((Dictionary<string, HashSet<int>> x) => x.ContainsKey(key.Collection));
		if (dictionary == null)
		{
			return false;
		}
		if (!dictionary.TryGetValue(key.Collection, out var value))
		{
			return false;
		}
		if (!_assortment.TryGetValue(new GIKey(value.First(), key.Collection), out var value2))
		{
			return false;
		}
		int id = key.ID;
		int min = Math.Max(0, id - value2.SaleTierDifference);
		return value.Any((int x) => x >= min && x <= id);
	}

	public void UpdateAssortment(ICollection<Item> mergeStoreItem, StoreSection section)
	{
		if (!_currentAssortment.ContainsKey(section))
		{
			_currentAssortment.Add(section, new Dictionary<string, HashSet<int>>());
		}
		foreach (Item item in mergeStoreItem.Where((Item x) => !x.Purchased.Value))
		{
			AddToAssortment(item, section);
			CreatSubscriptions(item, section);
			_assortment.Add(item.ItemKey, item);
			_onUpdate?.OnNext(item.ItemKey);
		}
	}

	private void CreatSubscriptions(Item item, StoreSection section)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		CompositeDisposable val = new CompositeDisposable();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Take<bool>(Observable.Where<bool>((IObservable<bool>)item.Purchased, (Func<bool, bool>)((bool x) => x)), 1), (Action<bool>)delegate
		{
			Remove(item, section);
		}), (ICollection<IDisposable>)val);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<ItemBuyRequest>(item.PurchasedRequest, (Action<ItemBuyRequest>)delegate(ItemBuyRequest request)
		{
			_onPurchasedRequest.OnNext(request.AddSection(section));
		}), (ICollection<IDisposable>)val);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IItem>(Observable.Take<IItem>(item.OnClear, 1), (Action<IItem>)delegate
		{
			Remove(item, section);
		}), (ICollection<IDisposable>)val);
		_subscriptions.Add(item.ItemKey, (IDisposable)val);
	}

	private void Remove(IItem item, StoreSection section)
	{
		_subscriptions[item.ItemKey].Dispose();
		_subscriptions.Remove(item.ItemKey);
		_assortment.Remove(item.ItemKey);
		RemoveToAssortment(item, section);
		_onUpdate?.OnNext(item.ItemKey);
	}

	private void RemoveToAssortment(IItem item, StoreSection section)
	{
		Dictionary<string, HashSet<int>> dictionary = _currentAssortment[section];
		if (dictionary.TryGetValue(item.ItemKey.Collection, out var value))
		{
			value.Remove(item.ItemKey.ID);
			if (value.Count == 0)
			{
				dictionary.Remove(item.ItemKey.Collection);
			}
			if (dictionary.Count == 0)
			{
				_currentAssortment.Remove(section);
			}
		}
	}

	private void AddToAssortment(IItem item, StoreSection section)
	{
		Dictionary<string, HashSet<int>> dictionary = _currentAssortment[section];
		if (!dictionary.TryGetValue(item.ItemKey.Collection, out var value))
		{
			value = new HashSet<int>();
			dictionary.Add(item.ItemKey.Collection, value);
		}
		value.Add(item.ItemKey.ID);
	}
}
