using System;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Constants;
using GreenT.Multiplier;
using UniRx;

namespace StripClub.Extensions;

public class EnergyLoadContainer : ILoadContainer, IDisposable
{
	private int _baseMaxEnergy;

	private TimeSpan _baseEnergyRestore;

	private readonly IConstants<int> _intConstants;

	private readonly IClock _clock;

	private readonly MultiplierManager _multiplierManager;

	private readonly CompositeDisposable _disposable = new CompositeDisposable();

	public DateTime LastRestore { get; set; }

	public EnergyLoadContainer(IClock clock, MultiplierManager multiplierManager, IConstants<int> intConstants)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_clock = clock;
		_intConstants = intConstants;
		_multiplierManager = multiplierManager;
	}

	public void OnLoad<T>(RestorableValue<T>.RestorableValueMemento restorableMemento) where T : struct, IComparable, IComparable<T>, IEquatable<T>
	{
		_baseEnergyRestore = TimeSpan.FromSeconds(_intConstants["time_energy_restore"]);
		_baseMaxEnergy = _intConstants["max_energy"];
		DateTime time = _clock.GetTime();
		LastRestore = ((restorableMemento.LastRestore > time) ? time : restorableMemento.LastRestore);
	}

	public int GetCap()
	{
		return (int)_multiplierManager.IncreaseEnergyMultipliers.Total.Factor.Value + _baseMaxEnergy;
	}

	public TimeSpan GetRechargeTime()
	{
		double num = Math.Clamp(_multiplierManager.IncreaseEnergyRechargeSpeedMultipliers.Total.Factor.Value, 1.0, double.MaxValue);
		return _baseEnergyRestore / num;
	}

	public T RestoreValueToCurrentTime<T>(T max, DateTime currentTime, TimeSpan restorePeriod, T amountPerTick) where T : IComparable, IComparable<T>, IEquatable<T>
	{
		T val = default(T);
		TimeSpan timeSpan = currentTime - LastRestore;
		if (timeSpan.Ticks < 0)
		{
			return val;
		}
		long num = timeSpan.Ticks / restorePeriod.Ticks;
		if (num == 0L)
		{
			return val;
		}
		T b = ExtensionMethods.Multiply(amountPerTick, num);
		val = ExtensionMethods.Add(val, b);
		if (max.CompareTo(val) <= 0)
		{
			LastRestore = currentTime;
		}
		else
		{
			LastRestore += restorePeriod.Multiply((double)num);
		}
		return val;
	}

	public void Dispose()
	{
		CompositeDisposable disposable = _disposable;
		if (disposable != null)
		{
			disposable.Dispose();
		}
	}
}
