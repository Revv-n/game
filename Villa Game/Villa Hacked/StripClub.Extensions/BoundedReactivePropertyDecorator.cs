using System;
using UniRx;

namespace StripClub.Extensions;

public class BoundedReactivePropertyDecorator<T> : ReactivePropertyDecorator<T> where T : struct, IComparable, IComparable<T>
{
	private T _max;

	private T _min;

	public T Max
	{
		get
		{
			return _max;
		}
		private set
		{
			_max = value;
			if (Property.Value.CompareTo(_max) >= 0)
			{
				Property.Value = value;
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
			if (Property.Value.CompareTo(_min) <= 0)
			{
				Property.Value = value;
			}
		}
	}

	public BoundedReactivePropertyDecorator(T max, T min, ReactiveProperty<T> property)
		: base(property)
	{
		Max = max;
		Min = min;
	}

	public bool SetBounds(T upperBound, T lowerBound = default(T))
	{
		if (upperBound.CompareTo(lowerBound) < 0)
		{
			return false;
		}
		Max = upperBound;
		Min = lowerBound;
		return true;
	}

	protected override void SetValue(T value)
	{
		if (value.CompareTo(Max) >= 0)
		{
			base.SetValue(Max);
		}
		else if (value.CompareTo(Min) <= 0)
		{
			base.SetValue(Min);
		}
		else
		{
			base.SetValue(value);
		}
	}
}
