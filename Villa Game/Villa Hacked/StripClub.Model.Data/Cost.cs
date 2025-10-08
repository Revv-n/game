using System;
using UnityEngine;

namespace StripClub.Model.Data;

[Serializable]
public struct Cost
{
	[field: SerializeField]
	public int Amount { get; private set; }

	[field: SerializeField]
	public CurrencyType Currency { get; private set; }

	public Cost(int amount, CurrencyType currency)
	{
		Amount = amount;
		Currency = currency;
	}

	public static Cost operator +(Cost cost, Cost addition)
	{
		if (addition.Amount == 0)
		{
			return cost;
		}
		if (cost.Currency.Equals(addition.Currency))
		{
			cost.Amount += addition.Amount;
			return cost;
		}
		throw new ArgumentException("Addition currency (" + addition.Currency.ToString() + ") must be equal cost currency (" + cost.Currency.ToString() + ")");
	}

	public static Cost operator -(Cost cost, Cost addition)
	{
		if (addition.Amount == 0)
		{
			return cost;
		}
		if (cost.Currency.Equals(addition.Currency))
		{
			cost.Amount -= addition.Amount;
			return cost;
		}
		throw new ArgumentException("Addition currency (" + addition.Currency.ToString() + ") must be equal cost currency (" + cost.Currency.ToString() + ")");
	}

	public static bool operator <=(Cost first, Cost second)
	{
		if (first.Currency == second.Currency)
		{
			return first.Amount <= second.Amount;
		}
		throw new ArgumentException();
	}

	public static bool operator >=(Cost first, Cost second)
	{
		if (first.Currency == second.Currency)
		{
			return first.Amount >= second.Amount;
		}
		throw new ArgumentException();
	}
}
