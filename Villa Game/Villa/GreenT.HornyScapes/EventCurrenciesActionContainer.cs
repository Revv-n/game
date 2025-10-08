using System;
using GreenT.HornyScapes.Analytics;
using StripClub.NewEvent.Model;
using UniRx;

namespace GreenT.HornyScapes;

public class EventCurrenciesActionContainer : ICurrenciesActionContainer, IDisposable
{
	private readonly EventCurrencyType currencyType;

	private EventWallet eventWallet;

	private readonly Subject<int> spendStream;

	private readonly Subject<int> addStream;

	public int Count => ReactiveCount.Value;

	public IReadOnlyReactiveProperty<int> ReactiveCount { get; }

	public CurrencyAmplitudeAnalytic.SourceType LastSourceType { get; private set; }

	public IObservable<int> OnSpend()
	{
		return spendStream;
	}

	public IObservable<int> OnAdd()
	{
		return addStream;
	}

	public EventCurrenciesActionContainer(EventCurrencyType currencyType, IReadOnlyReactiveProperty<int> reactiveProperty, EventWallet eventWallet)
	{
		this.currencyType = currencyType;
		ReactiveCount = reactiveProperty;
		this.eventWallet = eventWallet;
		spendStream = new Subject<int>();
		addStream = new Subject<int>();
	}

	public void UpdateWallet(EventWallet eventWallet)
	{
		this.eventWallet = eventWallet;
	}

	public bool TryAdd(int value, CurrencyAmplitudeAnalytic.SourceType sourceType = CurrencyAmplitudeAnalytic.SourceType.None)
	{
		int num;
		if (eventWallet != null)
		{
			num = (eventWallet.TryAdd(currencyType, value) ? 1 : 0);
			if (num != 0)
			{
				LastSourceType = sourceType;
				addStream.OnNext(value);
			}
		}
		else
		{
			num = 0;
		}
		return (byte)num != 0;
	}

	public bool TrySpend(int value)
	{
		int num;
		if (eventWallet != null)
		{
			num = (eventWallet.TrySpend(currencyType, value) ? 1 : 0);
			if (num != 0)
			{
				spendStream.OnNext(value);
			}
		}
		else
		{
			num = 0;
		}
		return (byte)num != 0;
	}

	public bool IsEnough(int value)
	{
		return Count >= value;
	}

	public void Reset()
	{
		eventWallet?.Reset(currencyType);
	}

	public void Dispose()
	{
		spendStream.Dispose();
		addStream.Dispose();
	}
}
