using System;
using System.Linq;
using System.Text;

namespace GreenT.Types;

[Serializable]
public struct CompositeIdentificator : IEquatable<CompositeIdentificator>
{
	public int[] Identificators;

	public int this[int i] => Identificators[i];

	public CompositeIdentificator(params int[] identificators)
	{
		Identificators = identificators;
	}

	public int GetParameter(int parameterNumber)
	{
		return Identificators[parameterNumber];
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder("[");
		int[] identificators = Identificators;
		foreach (int value in identificators)
		{
			stringBuilder.Append(value);
			stringBuilder.Append(';');
		}
		stringBuilder.Insert(stringBuilder.Length - 1, ']');
		return stringBuilder.ToString();
	}

	public string ToStringNoFrames()
	{
		return ToString().Replace("[", "").Replace("]", "").Trim();
	}

	public override int GetHashCode()
	{
		if (Identificators == null)
		{
			return 0;
		}
		return Identificators.Sum();
	}

	public override bool Equals(object obj)
	{
		if (obj is CompositeIdentificator other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(CompositeIdentificator other)
	{
		if (Identificators == null)
		{
			return true;
		}
		if (other.Identificators == null)
		{
			return Identificators == null;
		}
		if (Identificators.Length != other.Identificators.Length)
		{
			return false;
		}
		for (int i = 0; i != Identificators.Length; i++)
		{
			if (Identificators[i] != other.Identificators[i])
			{
				return false;
			}
		}
		return true;
	}

	public static bool operator ==(CompositeIdentificator x, CompositeIdentificator y)
	{
		return x.Equals(y);
	}

	public static bool operator !=(CompositeIdentificator x, CompositeIdentificator y)
	{
		return !x.Equals(y);
	}
}
