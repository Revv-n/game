using System;

namespace StripClub.Extensions;

public class SoftBoundedValue<T> : BoundedValue<T> where T : IComparable, IComparable<T>, IEquatable<T>
{
	public override T Value
	{
		get
		{
			return _current;
		}
		set
		{
			T current = _current;
			T val = ((_current.CompareTo(Max) > 0) ? _current : Max);
			T val2 = ((_current.CompareTo(Min) < 0) ? _current : Min);
			if (value.CompareTo(val) == 1)
			{
				_current = val;
			}
			else if (value.CompareTo(val2) == -1)
			{
				_current = val2;
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

	public override T Max
	{
		get
		{
			return _max;
		}
		protected set
		{
			_max = value;
		}
	}

	public override T Min
	{
		get
		{
			return _min;
		}
		protected set
		{
			_min = value;
		}
	}

	public SoftBoundedValue(T value, T upperBound, T lowerBound)
		: base(value, upperBound, lowerBound)
	{
	}

	public virtual void SetForce(T value)
	{
		bool num = !_current.Equals(value);
		_current = value;
		if (num)
		{
			Notify();
		}
	}
}
