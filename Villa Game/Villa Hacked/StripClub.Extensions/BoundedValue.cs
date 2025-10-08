using System;
using UnityEngine;

namespace StripClub.Extensions;

public class BoundedValue<T> where T : IComparable, IComparable<T>, IEquatable<T>
{
	protected T _current;

	protected T _max;

	protected T _min;

	public virtual T Value
	{
		get
		{
			return _current;
		}
		set
		{
			T current = _current;
			if (value.CompareTo(Max) == 1)
			{
				_current = Max;
			}
			else if (value.CompareTo(Min) == -1)
			{
				_current = Min;
			}
			else
			{
				_current = value;
			}
			if (!current.Equals(_current))
			{
				Notify();
			}
		}
	}

	public virtual T Max
	{
		get
		{
			return _max;
		}
		protected set
		{
			_max = value;
			if (Value.CompareTo(_max) >= 0)
			{
				Value = value;
			}
		}
	}

	public virtual T Min
	{
		get
		{
			return _min;
		}
		protected set
		{
			_min = value;
			if (Value.CompareTo(_min) <= 0)
			{
				Value = value;
			}
		}
	}

	public event Action OnGetMaxValue;

	public event Action OnGetMinValue;

	public event Action<T> OnValueChanged;

	public event Action<T, T> OnBoundsChanged;

	private BoundedValue(T upperBound, T lowerBound = default(T))
	{
		if (!SetBounds(upperBound, lowerBound))
		{
			SetBounds(lowerBound, upperBound);
			Debug.LogWarning("Bound parameters were switched, because upper bound must be greater then lower bound.");
		}
	}

	public BoundedValue(T value, T upperBound, T lowerBound)
		: this(upperBound, lowerBound)
	{
		Value = value;
	}

	private bool SetBounds(T upperBound, T lowerBound = default(T))
	{
		if (upperBound.CompareTo(lowerBound) < 0)
		{
			return false;
		}
		Max = upperBound;
		Min = lowerBound;
		return true;
	}

	public virtual bool UpdateBounds(T upperBound, T lowerBound = default(T))
	{
		if (!SetBounds(upperBound, lowerBound))
		{
			return false;
		}
		Notify();
		this.OnBoundsChanged?.Invoke(Min, Max);
		return true;
	}

	public bool CanSpend(T amount)
	{
		return ExtensionMethods.Add(amount, Min).CompareTo(Value) <= 0;
	}

	public void Add(T amount)
	{
		Value = ExtensionMethods.Add(Value, amount);
	}

	protected void OnValueReachedMaximum()
	{
		this.OnGetMaxValue?.Invoke();
	}

	protected void OnValueReachedMinimum()
	{
		this.OnGetMinValue?.Invoke();
	}

	protected void OnNewValue(T value)
	{
		this.OnValueChanged?.Invoke(value);
	}

	protected void Notify()
	{
		OnNewValue(_current);
		if (_current.CompareTo(Max) >= 0)
		{
			OnValueReachedMaximum();
		}
		else if (_current.CompareTo(Min) <= 0)
		{
			OnValueReachedMinimum();
		}
	}
}
