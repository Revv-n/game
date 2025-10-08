using System;
using StripClub.NewEvent.Save;
using UniRx;

namespace StripClub.NewEvent.Model;

[Serializable]
public class EventSimpleCurrency : IEventSavableState, IDisposable
{
	[Serializable]
	public class CurrencyMemento : EventMemento
	{
		public int Count;

		public CurrencyMemento(EventSimpleCurrency pocketRepository)
			: base(pocketRepository)
		{
			Count = pocketRepository.Count.Value;
		}
	}

	private readonly int initialValue;

	private readonly string uniqueKey;

	private readonly EventCurrencyType currencyType;

	public ReactiveProperty<int> Count { get; }

	public EventCurrencyType CurrencyType => currencyType;

	public EventSimpleCurrency(EventCurrencyType currencyType, int initialValue)
	{
		this.currencyType = currencyType;
		this.initialValue = initialValue;
		Count = new ReactiveProperty<int>(initialValue);
		uniqueKey = $"event_currency_{currencyType}";
	}

	public string UniqueKey()
	{
		return uniqueKey;
	}

	public void ResetToDefaultState()
	{
		Count.Value = initialValue;
	}

	public EventMemento SaveState()
	{
		return new CurrencyMemento(this);
	}

	public void LoadState(EventMemento memento)
	{
		CurrencyMemento currencyMemento = (CurrencyMemento)memento;
		Count.Value = currencyMemento.Count;
	}

	public void Dispose()
	{
		Count.Dispose();
		ResetToDefaultState();
	}
}
