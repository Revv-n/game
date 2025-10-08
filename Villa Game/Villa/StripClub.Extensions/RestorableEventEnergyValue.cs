using System;
using System.Runtime.Serialization;
using GreenT.Data;
using StripClub.UI;
using UniRx;

namespace StripClub.Extensions;

[MementoHolder]
public class RestorableEventEnergyValue<T> : SoftBoundedValue<T>, ISavableState, IDisposable where T : IComparable, IComparable<T>, IEquatable<T>
{
	[Serializable]
	public class RestorableEventEnergyValueMemento : Memento
	{
		public T Value { get; private set; }

		public T Min { get; private set; }

		public T Max { get; private set; }

		public DateTime LastRestore { get; private set; }

		public RestorableEventEnergyValueMemento(RestorableEventEnergyValue<T> restorableVariable)
			: base(restorableVariable)
		{
			Value = restorableVariable.Value;
			Min = restorableVariable.Min;
			Max = restorableVariable.Max;
			LastRestore = restorableVariable.LastRestoreDate;
		}
	}

	private IDisposable timerStream;

	private IDisposable applicationPauseWatcherStream;

	private bool isWatching;

	private readonly string key;

	public override T Value
	{
		set
		{
			base.Value = value;
			ManageWatch(resetLastRestoreDate: true);
		}
	}

	public T AmountPerTick { get; set; }

	public TimeSpan RestorePeriod { get; set; }

	public DateTime LastRestoreDate { get; protected set; }

	public TimeSpan CheckFrequency { get; set; }

	public Func<DateTime> GetTime { get; set; }

	public TimeSpan TimeUntilNextRestore
	{
		get
		{
			if (!isWatching)
			{
				return TimeSpan.Zero;
			}
			return LastRestoreDate + RestorePeriod - GetTime();
		}
	}

	public GenericTimer Timer { get; private set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public RestorableEventEnergyValue(T initValue, Func<DateTime> getTime, DateTime lastTick, TimeSpan restorePeriod, TimeSpan checkFrequency, T amountPerTick, T upperBound, T lowerBound = default(T), string key = null)
		: base(initValue, upperBound, lowerBound)
	{
		if (!typeof(T).IsSerializable && !typeof(ISerializable).IsAssignableFrom(typeof(T)))
		{
			throw new InvalidOperationException("A serializable Type is required");
		}
		AmountPerTick = amountPerTick;
		this.key = key;
		RestorePeriod = restorePeriod;
		LastRestoreDate = lastTick;
		CheckFrequency = checkFrequency;
		GetTime = getTime;
		SetupTimer();
		ManageWatch(resetLastRestoreDate: false);
		TrackApplicationPause();
	}

	private void SetupTimer()
	{
		Timer?.Dispose();
		Timer = new GenericTimer();
		Timer.UpdatePeriod = CheckFrequency;
		timerStream = Timer.OnTimeIsUp.Subscribe(delegate
		{
			UnitIncrease();
		}, delegate(Exception ex)
		{
			throw ex;
		}, delegate
		{
			isWatching = false;
		});
	}

	protected void StartWatch()
	{
		if (Value.CompareTo(Max) == -1)
		{
			isWatching = true;
			Timer.Start(TimeUntilNextRestore);
		}
	}

	protected virtual void UnitIncrease()
	{
		LastRestoreDate = GetTime();
		Add(AmountPerTick);
		if (Value.CompareTo(Max) == -1)
		{
			Timer.Start(RestorePeriod);
		}
		else
		{
			isWatching = false;
		}
	}

	private void TrackApplicationPause()
	{
		applicationPauseWatcherStream?.Dispose();
		applicationPauseWatcherStream = MainThreadDispatcher.OnApplicationPauseAsObservable().Subscribe(OnApplicationPause);
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause && isWatching)
		{
			RestoreValueToCurrentTime(GetTime());
			if (Value.CompareTo(Max) == -1)
			{
				Timer.Start(TimeUntilNextRestore);
			}
			else
			{
				StopWatch();
			}
		}
	}

	public virtual void ExtraIncrease(TimeSpan timeGap)
	{
		T restoreAmount = GetRestoreAmount(timeGap);
		LastRestoreDate += RestorePeriod - new TimeSpan(timeGap.Ticks % RestorePeriod.Ticks);
		Add(restoreAmount);
	}

	public T GetRestoreAmount(TimeSpan timeGap)
	{
		long num = timeGap.Ticks / RestorePeriod.Ticks;
		_ = timeGap.Ticks % RestorePeriod.Ticks;
		if ((GetTime() - LastRestoreDate - timeGap).Ticks >= 0)
		{
			num++;
		}
		T result = ExtensionMethods.Multiply(AmountPerTick, num);
		T val = ExtensionMethods.Substract(Max, Value);
		if (result.CompareTo(val) <= 0)
		{
			return result;
		}
		return val;
	}

	protected virtual void RestoreValueToCurrentTime(DateTime currentTime)
	{
		TimeSpan timeSpan = currentTime - LastRestoreDate;
		if (timeSpan.Ticks < 0)
		{
			return;
		}
		long num = timeSpan.Ticks / RestorePeriod.Ticks;
		if (num != 0L)
		{
			T b = ExtensionMethods.Multiply(AmountPerTick, num);
			base.Value = ExtensionMethods.Add(Value, b);
			if (Max.CompareTo(Value) <= 0)
			{
				LastRestoreDate = currentTime;
			}
			else
			{
				LastRestoreDate += RestorePeriod.Multiply(num);
			}
		}
	}

	protected virtual void StopWatch()
	{
		Timer.Stop();
		isWatching = false;
	}

	private void ManageWatch(bool resetLastRestoreDate)
	{
		if (GetTime == null)
		{
			return;
		}
		if (isWatching && Value.CompareTo(Max) >= 0)
		{
			StopWatch();
		}
		else if (!isWatching && Value.CompareTo(Max) == -1)
		{
			DateTime dateTime = GetTime();
			if (resetLastRestoreDate && LastRestoreDate < dateTime)
			{
				LastRestoreDate = dateTime;
			}
			else
			{
				RestoreValueToCurrentTime(dateTime);
			}
			StartWatch();
		}
	}

	public override bool UpdateBounds(T upperBound, T lowerBound = default(T))
	{
		if (!base.UpdateBounds(upperBound, lowerBound))
		{
			return false;
		}
		ManageWatch(resetLastRestoreDate: true);
		return true;
	}

	public override void SetForce(T value)
	{
		base.SetForce(value);
		ManageWatch(resetLastRestoreDate: true);
	}

	public string UniqueKey()
	{
		if (key != null)
		{
			return key;
		}
		return string.Empty;
	}

	public Memento SaveState()
	{
		return new RestorableEventEnergyValueMemento(this);
	}

	public void LoadState(Memento memento)
	{
		if (isWatching)
		{
			StopWatch();
		}
		RestorableEventEnergyValueMemento restorableEventEnergyValueMemento = (RestorableEventEnergyValueMemento)memento;
		base.UpdateBounds(restorableEventEnergyValueMemento.Max, restorableEventEnergyValueMemento.Min);
		base.SetForce(restorableEventEnergyValueMemento.Value);
		DateTime dateTime = GetTime();
		LastRestoreDate = ((restorableEventEnergyValueMemento.LastRestore > dateTime) ? dateTime : restorableEventEnergyValueMemento.LastRestore);
		ManageWatch(resetLastRestoreDate: false);
		Notify();
	}

	public void Dispose()
	{
		applicationPauseWatcherStream?.Dispose();
		timerStream?.Dispose();
		Timer.Dispose();
	}
}
