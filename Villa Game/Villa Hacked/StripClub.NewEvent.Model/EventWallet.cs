using System;
using StripClub.NewEvent.Data;
using UniRx;

namespace StripClub.NewEvent.Model;

[Serializable]
public class EventWallet : IEventWallet, IDisposableEventInformation, IDisposable
{
	private EventSimpleCurrencyDictionary currenciesDict = new EventSimpleCurrencyDictionary();

	public EventSimpleCurrency this[EventCurrencyType type] => currenciesDict[type];

	public void ForceSet(EventSimpleCurrency simpleCurrency)
	{
		currenciesDict[simpleCurrency.CurrencyType] = simpleCurrency;
	}

	public bool IsEnough(EventCurrencyType currencyType, int cost)
	{
		if (!currenciesDict.TryGetValue(currencyType, out var value))
		{
			return false;
		}
		return value.Count.Value >= cost;
	}

	public bool TryAdd(EventCurrencyType currencyType, int cost)
	{
		if (!currenciesDict.TryGetValue(currencyType, out var value))
		{
			return false;
		}
		ReactiveProperty<int> count = value.Count;
		count.Value += cost;
		return true;
	}

	public bool TrySpend(EventCurrencyType currencyType, int cost)
	{
		if (!IsEnough(currencyType, cost))
		{
			return false;
		}
		ReactiveProperty<int> count = currenciesDict[currencyType].Count;
		count.Value -= cost;
		return true;
	}

	public IReactiveProperty<int> Get(EventCurrencyType type)
	{
		return (IReactiveProperty<int>)(object)currenciesDict[type].Count;
	}

	public void Dispose()
	{
		foreach (EventSimpleCurrency value in currenciesDict.Values)
		{
			value.Dispose();
		}
		currenciesDict.Clear();
	}

	public void Reset(EventCurrencyType currencyType)
	{
		if (currenciesDict.TryGetValue(currencyType, out var value))
		{
			value.Count.Value = 0;
		}
	}
}
