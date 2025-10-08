using System;
using System.Collections.Generic;
using System.Text;

namespace GreenT.Model.Reactive;

public struct CollectionAddERangEvent<T> : IEquatable<CollectionAddERangEvent<T>>
{
	public IEnumerable<T> Items { get; private set; }

	public CollectionAddERangEvent(IEnumerable<T> items)
	{
		this = default(CollectionAddERangEvent<T>);
		Items = items;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder("Values: ");
		foreach (T item in Items)
		{
			stringBuilder.Append(item);
			stringBuilder.Append(',');
		}
		return stringBuilder.ToString();
	}

	public bool Equals(CollectionAddERangEvent<T> other)
	{
		return EqualityComparer<IEnumerable<T>>.Default.Equals(Items, other.Items);
	}

	public override int GetHashCode()
	{
		return -604923257 + EqualityComparer<IEnumerable<T>>.Default.GetHashCode(Items);
	}
}
