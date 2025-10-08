using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Analytics;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Data;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes;

public class CurrencyProcessor : ICurrencyProcessor
{
	private readonly WalletProvider _walletProvider;

	public CurrencyProcessor(WalletProvider walletProvider)
	{
		_walletProvider = walletProvider;
	}

	public bool TrySpent(Cost cost, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		return TrySpent(cost.Currency, cost.Amount, compositeIdentificator);
	}

	public bool TrySpent(CurrencyType type, int value, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		if (_walletProvider.TryGetContainer(type, out var container, compositeIdentificator))
		{
			return container.TrySpend(value);
		}
		return false;
	}

	public bool IsEnough(Cost cost, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		return IsEnough(cost.Currency, cost.Amount, compositeIdentificator);
	}

	public bool IsEnough(CurrencyType type, int amount, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		if (_walletProvider.TryGetContainer(type, out var container, compositeIdentificator))
		{
			return container.IsEnough(amount);
		}
		return false;
	}

	public bool TryAdd(CurrencyType type, int amount, CurrencyAmplitudeAnalytic.SourceType sourceType = CurrencyAmplitudeAnalytic.SourceType.None, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		if (_walletProvider.TryGetContainer(type, out var container, compositeIdentificator))
		{
			return container.TryAdd(amount, sourceType);
		}
		return false;
	}

	public bool TryGetCount(CurrencyType type, out int count, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		count = 0;
		if (!_walletProvider.TryGetContainer(type, out var container, compositeIdentificator))
		{
			return false;
		}
		if (container.Count < 0)
		{
			return false;
		}
		count = container.Count;
		return true;
	}

	public int GetCount(CurrencyType type, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		if (!TryGetCount(type, out var count, compositeIdentificator))
		{
			Debug.LogError(new Exception($"Unable to find out the amount of {type} currency"));
		}
		return count;
	}

	public bool HasContainer(CurrencyType type, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		ICurrenciesActionContainer container;
		return _walletProvider.TryGetContainer(type, out container, compositeIdentificator);
	}

	public bool TryGetCountReactiveProperty(CurrencyType type, out IReadOnlyReactiveProperty<int> count, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		count = null;
		if (!_walletProvider.TryGetContainer(type, out var container, compositeIdentificator))
		{
			return false;
		}
		count = container.ReactiveCount;
		return count != null;
	}

	public IReadOnlyReactiveProperty<int> GetCountReactiveProperty(CurrencyType type, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		if (!TryGetCountReactiveProperty(type, out var count, compositeIdentificator))
		{
			Debug.LogError(new Exception($"Unable to find out the amount of {type} currency"));
		}
		return count;
	}

	public IDisposable GetAddStream(CurrencyType type, Action<int> listener, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		if (!_walletProvider.TryGetContainer(type, out var container, compositeIdentificator))
		{
			Debug.LogError(new Exception($"Unable to find out the AddStream of {type} currency"));
		}
		IObservable<int> observable = container.OnAdd();
		if (observable != null)
		{
			return ObservableExtensions.Subscribe<int>(observable, listener);
		}
		Debug.LogError(new Exception($"This currency {type} does not support the flow AddStream"));
		return null;
	}

	public IDisposable GetSpendStream(CurrencyType type, Action<int> listener, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		IObservable<int> spendObservable = GetSpendObservable(type, compositeIdentificator);
		if (spendObservable != null)
		{
			return ObservableExtensions.Subscribe<int>(spendObservable, listener);
		}
		Debug.LogError(new Exception($"This currency {type} does not support the flow SpendStream"));
		return null;
	}

	public IObservable<int> GetSpendObservable(CurrencyType type, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		if (!_walletProvider.TryGetContainer(type, out var container, compositeIdentificator))
		{
			Debug.LogError(new Exception($"Unable to find out the SpendStream of {type} currency"));
		}
		return container.OnSpend();
	}

	public IDisposable GetChangeStream(CurrencyType type, Action<int> listener, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		if (!_walletProvider.TryGetContainer(type, out var container, compositeIdentificator))
		{
			Debug.LogError(new Exception($"Unable to find out the SpendStream of {type} currency"));
		}
		CompositeDisposable val = new CompositeDisposable();
		IObservable<int> observable = container.OnSpend();
		IObservable<int> observable2 = container.OnAdd();
		if (observable != null)
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(observable, listener), (ICollection<IDisposable>)val);
		}
		if (observable2 != null)
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(observable2, listener), (ICollection<IDisposable>)val);
		}
		if (observable == null && observable2 == null)
		{
			Debug.LogError(new Exception($"This currency {type} does not support the flow SpendStream"));
		}
		return (IDisposable)val;
	}

	public bool TryReset(CurrencyType type, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		if (!_walletProvider.TryGetContainer(type, out var container, compositeIdentificator))
		{
			return false;
		}
		container.Reset();
		return true;
	}
}
