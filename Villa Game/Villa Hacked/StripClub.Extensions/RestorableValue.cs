using System;
using System.Runtime.Serialization;
using GreenT.Data;
using StripClub.UI;
using UniRx;

namespace StripClub.Extensions;

[MementoHolder]
public class RestorableValue<T> : SoftBoundedValue<T>, ISavableState, IDisposable where T : struct, IComparable, IComparable<T>, IEquatable<T>
{
	[Serializable]
	public class RestorableValueMemento : Memento
	{
		public T Value { get; private set; }

		public T Min { get; private set; }

		public T Max { get; private set; }

		public DateTime LastRestore { get; private set; }

		public RestorableValueMemento(RestorableValue<T> restorableVariable)
			: base(restorableVariable)
		{
			Value = restorableVariable.Value;
			Min = restorableVariable.Min;
			Max = restorableVariable.Max;
			LastRestore = restorableVariable.LastRestoreDate;
		}
	}

	private bool isWatching;

	private readonly string key;

	private EnergyLoadContainer _loadContainer;

	private IDisposable timerStream;

	private IDisposable applicationPauseWatcherStream;

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

	public DateTime LastRestoreDate
	{
		get
		{
			return _loadContainer.LastRestore;
		}
		private set
		{
			_loadContainer.LastRestore = value;
		}
	}

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

	public Subject<Unit> OnUpdate { get; } = new Subject<Unit>();


	public SavableStatePriority Priority => SavableStatePriority.Base;

	public RestorableValue(T initValue, Func<DateTime> getTime, DateTime lastTick, TimeSpan restorePeriod, TimeSpan checkFrequency, T amountPerTick, T upperBound, T lowerBound = default(T), string key = null, EnergyLoadContainer loadContainer = null)
		: base(initValue, upperBound, lowerBound)
	{
		if (!typeof(T).IsSerializable && !typeof(ISerializable).IsAssignableFrom(typeof(T)))
		{
			throw new InvalidOperationException("A serializable Type is required");
		}
		GetTime = getTime;
		_loadContainer = loadContainer;
		AmountPerTick = amountPerTick;
		this.key = key;
		RestorePeriod = restorePeriod;
		LastRestoreDate = lastTick;
		CheckFrequency = checkFrequency;
		SetupTimer();
		ManageWatch(resetLastRestoreDate: false);
		TrackApplicationPause();
	}

	private void SetupTimer()
	{
		Timer?.Dispose();
		Timer = new GenericTimer();
		Timer.UpdatePeriod = CheckFrequency;
		timerStream = ObservableExtensions.Subscribe<GenericTimer>(Timer.OnTimeIsUp, (Action<GenericTimer>)delegate
		{
			UnitIncrease();
		}, (Action<Exception>)delegate(Exception ex)
		{
			throw ex;
		}, (Action)delegate
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

	public void ChangeRestorePeriod(TimeSpan restorePeriod)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		RestorePeriod = restorePeriod;
		if (isWatching)
		{
			if (Timer.TimeLeft > RestorePeriod)
			{
				Timer.TimeLeft = RestorePeriod;
			}
			OnUpdate.OnNext(Unit.Default);
		}
	}

	private void TrackApplicationPause()
	{
		applicationPauseWatcherStream?.Dispose();
		applicationPauseWatcherStream = ObservableExtensions.Subscribe<bool>(MainThreadDispatcher.OnApplicationPauseAsObservable(), (Action<bool>)OnApplicationPause);
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause && isWatching)
		{
			DateTime currentTime = GetTime();
			RestoreValueToCurrentTime(currentTime);
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

	public virtual void RestoreValueToCurrentTime(DateTime currentTime)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		T b = _loadContainer.RestoreValueToCurrentTime(Max, currentTime, RestorePeriod, AmountPerTick);
		base.Value = ExtensionMethods.Add(Value, b);
		OnUpdate.OnNext(default(Unit));
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
			else if (LastRestoreDate < dateTime)
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

	public bool UpdateBoundsInfluenced(T upperBound, T lowerBound = default(T))
	{
		if (!base.UpdateBounds(upperBound, lowerBound))
		{
			return false;
		}
		ManageWatch(resetLastRestoreDate: false);
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
		return new RestorableValueMemento(this);
	}

	public void LoadState(Memento memento)
	{
		if (isWatching)
		{
			StopWatch();
		}
		RestorableValueMemento restorableValueMemento = (RestorableValueMemento)memento;
		base.SetForce(restorableValueMemento.Value);
		_loadContainer.OnLoad(restorableValueMemento);
		ChangeRestorePeriod(_loadContainer.GetRechargeTime());
		UpdateBoundsInfluenced((T)(object)_loadContainer.GetCap(), Min);
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
