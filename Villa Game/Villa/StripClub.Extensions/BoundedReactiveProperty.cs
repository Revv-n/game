using System;
using System.Runtime.Serialization;
using UniRx;
using UnityEngine;

namespace StripClub.Extensions;

[Serializable]
public class BoundedReactiveProperty<T> : ReactiveProperty<T>, ISerializable where T : struct, IComparable, IComparable<T>, IEquatable<T>
{
	private T _max;

	private T _min;

	protected ISubject<T> maxBoundChanged = new Subject<T>();

	protected ISubject<T> minBoundChanged = new Subject<T>();

	public IObservable<T> MaxBoundChanged => maxBoundChanged.AsObservable();

	public IObservable<T> MinBoundChanged => minBoundChanged.AsObservable();

	public T Max
	{
		get
		{
			return _max;
		}
		private set
		{
			_max = value;
			if (base.Value.CompareTo(_max) >= 0)
			{
				base.Value = value;
			}
		}
	}

	public T Min
	{
		get
		{
			return _min;
		}
		private set
		{
			_min = value;
			if (base.Value.CompareTo(_min) <= 0)
			{
				base.Value = value;
			}
		}
	}

	public event Action OnGetMaxValue;

	public event Action OnGetMinValue;

	public BoundedReactiveProperty(T value, T max, T min = default(T))
	{
		if (!SetBounds(max, min))
		{
			SetBounds(min, max);
			Debug.LogWarning("Bound parameters were switched, because upper bound must be greater then lower bound.");
		}
		base.Value = value;
	}

	public bool SetBounds(T upperBound, T lowerBound = default(T))
	{
		if (upperBound.CompareTo(lowerBound) < 0)
		{
			return false;
		}
		T min = _min;
		T max = _max;
		Max = upperBound;
		Min = lowerBound;
		if (!Max.Equals(max))
		{
			maxBoundChanged.OnNext(Max);
		}
		if (!Min.Equals(min))
		{
			minBoundChanged.OnNext(Min);
		}
		return true;
	}

	protected override void SetValue(T value)
	{
		if (value.CompareTo(Max) >= 0)
		{
			base.SetValue(Max);
			this.OnGetMaxValue?.Invoke();
		}
		else if (value.CompareTo(Min) <= 0)
		{
			base.SetValue(Min);
			this.OnGetMinValue?.Invoke();
		}
		else
		{
			base.SetValue(value);
		}
	}

	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("Value", base.Value);
		info.AddValue("Min", Min);
		info.AddValue("Max", Max);
	}

	protected BoundedReactiveProperty(SerializationInfo info, StreamingContext context)
	{
		T lowerBound = (T)info.GetValue("Min", typeof(T));
		T upperBound = (T)info.GetValue("Max", typeof(T));
		T val = (T)info.GetValue("Value", typeof(T));
		base.SetValue(val);
		SetBounds(upperBound, lowerBound);
	}
}
