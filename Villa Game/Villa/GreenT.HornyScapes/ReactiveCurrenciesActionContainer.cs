using System;
using GreenT.HornyScapes.Analytics;
using GreenT.Types;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes;

public class ReactiveCurrenciesActionContainer : ICurrenciesActionContainer, IDisposable
{
	private readonly CurrencyType currencyType;

	private readonly Currencies mainBalance;

	private readonly CompositeIdentificator _currencyIdentificator;

	private readonly Subject<int> spendStream;

	private readonly Subject<int> addStream;

	public int Count => ReactiveCount.Value;

	public IReadOnlyReactiveProperty<int> ReactiveCount => mainBalance[currencyType, _currencyIdentificator].Count;

	public CurrencyAmplitudeAnalytic.SourceType LastSourceType { get; private set; }

	public IObservable<int> OnSpend()
	{
		return spendStream.AsObservable();
	}

	public IObservable<int> OnAdd()
	{
		return addStream.AsObservable();
	}

	public ReactiveCurrenciesActionContainer(CurrencyType currencyType, Currencies mainBalance, CompositeIdentificator currencyIdentificator = default(CompositeIdentificator))
	{
		this.currencyType = currencyType;
		this.mainBalance = mainBalance;
		spendStream = new Subject<int>();
		addStream = new Subject<int>();
		_currencyIdentificator = currencyIdentificator;
	}

	public bool TryAdd(int value, CurrencyAmplitudeAnalytic.SourceType sourceType = CurrencyAmplitudeAnalytic.SourceType.None)
	{
		bool num = mainBalance.TryAdd(currencyType, value, _currencyIdentificator);
		if (num)
		{
			addStream.OnNext(value);
		}
		return num;
	}

	public bool TrySpend(int value)
	{
		bool num = mainBalance.Spend(currencyType, value, _currencyIdentificator);
		if (num)
		{
			spendStream.OnNext(value);
		}
		return num;
	}

	public bool IsEnough(int value)
	{
		return Count >= value;
	}

	public void Reset()
	{
		mainBalance.Reset(currencyType, _currencyIdentificator);
	}

	public void Dispose()
	{
		spendStream.Dispose();
		addStream.Dispose();
	}
}
