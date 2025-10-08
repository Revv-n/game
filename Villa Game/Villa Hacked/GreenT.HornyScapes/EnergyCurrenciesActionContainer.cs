using System;
using GreenT.HornyScapes.Analytics;
using GreenT.Types;
using StripClub.Extensions;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes;

public class EnergyCurrenciesActionContainer : ICurrenciesActionContainer, IDisposable
{
	private readonly RestorableValue<int> energy;

	private readonly Currencies mainBalance;

	private readonly Subject<int> spendStream;

	private readonly Subject<int> addStream;

	public int Count => energy.Value;

	public IReadOnlyReactiveProperty<int> ReactiveCount => (IReadOnlyReactiveProperty<int>)(object)mainBalance[CurrencyType.Energy, default(CompositeIdentificator)].Count;

	public CurrencyAmplitudeAnalytic.SourceType LastSourceType { get; private set; }

	public IObservable<int> OnSpend()
	{
		return (IObservable<int>)spendStream;
	}

	public IObservable<int> OnAdd()
	{
		return (IObservable<int>)addStream;
	}

	public EnergyCurrenciesActionContainer(RestorableValue<int> energy, Currencies mainBalance)
	{
		this.energy = energy;
		this.mainBalance = mainBalance;
		spendStream = new Subject<int>();
		addStream = new Subject<int>();
	}

	public bool TryAdd(int value, CurrencyAmplitudeAnalytic.SourceType sourceType = CurrencyAmplitudeAnalytic.SourceType.None)
	{
		if (value < 0)
		{
			return false;
		}
		mainBalance.OldEnergy.Count.Value = Count + value;
		energy.SetForce(Count + value);
		addStream.OnNext(value);
		return true;
	}

	public bool TrySpend(int value)
	{
		if (Count <= 0 || value < 0 || value > Count)
		{
			return false;
		}
		mainBalance.OldEnergy.Count.Value = Count - value;
		energy.SetForce(Count - value);
		spendStream.OnNext(value);
		return true;
	}

	public bool IsEnough(int value)
	{
		return Count >= value;
	}

	public void Reset()
	{
		mainBalance.OldEnergy.Count.Value = 0;
		energy.SetForce(0);
	}

	public void Dispose()
	{
		spendStream.Dispose();
		addStream.Dispose();
	}
}
