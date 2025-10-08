using System;
using UnityEngine;

[Serializable]
public struct MinMaxInt
{
	public int min;

	public int max;

	public int Distance => max - min;

	public MinMaxInt(int max)
	{
		min = 0;
		this.max = max;
	}

	public MinMaxInt(int min, int max)
		: this(max)
	{
		this.min = min;
	}

	public int GetRandom()
	{
		return UnityEngine.Random.Range(min, max);
	}

	public bool Contains(int value)
	{
		if (value >= min)
		{
			return value <= max;
		}
		return false;
	}
}
