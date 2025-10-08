using System;
using UnityEngine;

[Serializable]
public struct MinMax
{
	[SerializeField]
	private float min;

	[SerializeField]
	private float max;

	public float Min
	{
		get
		{
			return min;
		}
		set
		{
			min = value;
		}
	}

	public float Max
	{
		get
		{
			return max;
		}
		set
		{
			max = value;
		}
	}

	public float Distance => Max - Min;

	public float Centre => (Max - Min) / 2f;

	public float Random => UnityEngine.Random.Range(Min, Max);

	public static MinMax One => new MinMax(0f, 1f);

	public MinMax(float max)
	{
		min = 0f;
		this.max = max;
	}

	public MinMax(float min, float max)
		: this(max)
	{
		this.min = min;
	}

	public float GetPercent(float percent)
	{
		return Min + Distance * percent;
	}

	public bool Contains(float value)
	{
		if (value >= min)
		{
			return value <= max;
		}
		return false;
	}

	public static MinMax New(float min, float max)
	{
		return new MinMax(min, max);
	}

	public static MinMax AnyToMax(float max)
	{
		return new MinMax(float.MinValue, max);
	}

	public static MinMax ZeroToMax(float max)
	{
		return new MinMax(0f, max);
	}

	public static MinMax MoreThen(float min)
	{
		return new MinMax(min, float.MaxValue);
	}
}
