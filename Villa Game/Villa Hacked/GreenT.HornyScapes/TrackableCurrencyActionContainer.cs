using System;
using GreenT.HornyScapes.Analytics;
using GreenT.Types;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes;

public sealed class TrackableCurrencyActionContainer : ICurrenciesActionContainer, IDisposable, ITrackableCurrencyContainer
{
	private readonly CurrencyType _currencyType;

	private readonly CompositeIdentificator _compositeIdentificator;

	private readonly Currencies _mainBalance;

	private readonly Subject<int> _spendStream;

	private readonly Subject<int> _addStream;

	public CurrencyAmplitudeAnalytic.SourceType LastSourceType { get; private set; }

	public int Count => ReactiveCount.Value;

	public IReadOnlyReactiveProperty<int> ReactiveCount => (IReadOnlyReactiveProperty<int>)(object)_mainBalance[_currencyType, _compositeIdentificator].Count;

	public TrackableCurrencyActionContainer(CurrencyType currencyType, Currencies mainBalance, CompositeIdentificator compositeIdentificator)
	{
		_currencyType = currencyType;
		_compositeIdentificator = compositeIdentificator;
		_mainBalance = mainBalance;
		_spendStream = new Subject<int>();
		_addStream = new Subject<int>();
	}

	public CurrencyType GetCurrencyType()
	{
		return _currencyType;
	}

	public IObservable<int> OnSpend()
	{
		return Observable.AsObservable<int>((IObservable<int>)_spendStream);
	}

	public IObservable<int> OnAdd()
	{
		return Observable.AsObservable<int>((IObservable<int>)_addStream);
	}

	public bool TryAdd(int value, CurrencyAmplitudeAnalytic.SourceType sourceType = CurrencyAmplitudeAnalytic.SourceType.None)
	{
		bool num = _mainBalance.TryAdd(_currencyType, value, _compositeIdentificator);
		if (num)
		{
			LastSourceType = sourceType;
			_addStream.OnNext(value);
		}
		return num;
	}

	public bool TrySpend(int value)
	{
		bool num = _mainBalance.Spend(_currencyType, value, _compositeIdentificator);
		if (num)
		{
			_spendStream.OnNext(value);
		}
		return num;
	}

	public bool IsEnough(int value)
	{
		return Count >= value;
	}

	public void Reset()
	{
		_mainBalance.Reset(_currencyType, _compositeIdentificator);
	}

	public void Dispose()
	{
		_spendStream.Dispose();
		_addStream.Dispose();
	}
}
