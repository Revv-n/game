using System;

namespace StripClub.Model;

public class EqualityLocker<T> : Locker where T : IComparable<T>
{
	private T initValue;

	public T Value { get; }

	public Restriction Restrictor { get; }

	public EqualityLocker(T targetValue, Restriction restrictor)
	{
		Value = targetValue;
		Restrictor = restrictor;
	}

	public EqualityLocker(T currentValue, T targetValue, Restriction restrictor)
		: this(targetValue, restrictor)
	{
		initValue = currentValue;
		Set(currentValue);
	}

	public override void Initialize()
	{
		Set(initValue);
	}

	public virtual void Set(T currentValue)
	{
		switch (Restrictor)
		{
		case Restriction.Min:
			isOpen.Value = Value.CompareTo(currentValue) <= 0;
			break;
		case Restriction.Max:
			isOpen.Value = Value.CompareTo(currentValue) > 0;
			break;
		case Restriction.Equal:
			isOpen.Value = Value.CompareTo(currentValue) == 0;
			break;
		default:
			throw new ArgumentOutOfRangeException("There is no behaviour for type: " + Restrictor);
		}
	}
}
