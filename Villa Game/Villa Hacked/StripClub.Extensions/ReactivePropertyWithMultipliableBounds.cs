using System;
using System.Runtime.Serialization;
using GreenT.Multiplier;
using UniRx;

namespace StripClub.Extensions;

[Serializable]
public class ReactivePropertyWithMultipliableBounds<T> : BoundedReactiveProperty<T>, ISerializable where T : struct, IComparable, IComparable<T>, IEquatable<T>
{
	private T serializableMax;

	public readonly IOrderedCompositeMultiplier Multiplier = new OrderedCompositeMultiplier(OrderedCompositeMultiplier.DefaultComposite);

	private IDisposable multiplierStream;

	public ReactivePropertyWithMultipliableBounds(T value, T max, T min = default(T))
		: base(value, max, min)
	{
		serializableMax = max;
		multiplierStream = ObservableExtensions.Subscribe<double>(Observable.TakeWhile<double>((IObservable<double>)Multiplier.Factor, (Func<double, bool>)((double _) => this != null)), (Action<double>)MultiplyUpperBoundOnFactor);
	}

	protected void MultiplyUpperBoundOnFactor<K>(K value) where K : struct, IComparable, IComparable<K>, IEquatable<K>
	{
		SetBounds(Multiply(serializableMax, value));
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("Value", ((ReactiveProperty<T>)this).Value);
		info.AddValue("Min", base.Min);
		info.AddValue("Max", serializableMax);
	}

	public bool CanSpend(T amount)
	{
		if (Add(amount, base.Min).CompareTo(((ReactiveProperty<T>)this).Value) > 0)
		{
			return false;
		}
		return true;
	}

	protected virtual void Add(T amount)
	{
		((ReactiveProperty<T>)this).Value = ExtensionMethods.Add(((ReactiveProperty<T>)this).Value, amount);
	}

	protected static T Add(T a, T b)
	{
		return ExtensionMethods.Add(a, b);
	}

	protected static T Multiply<K>(T a, K b) where K : struct, IComparable, IComparable<K>, IEquatable<K>
	{
		return ExtensionMethods.Multiply(a, b);
	}
}
