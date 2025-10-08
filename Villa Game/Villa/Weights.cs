using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Weights
{
	public static WeightNode<T> GetWeightNode<T>(IList<WeightNode<T>> list)
	{
		return BaseSerch(list);
	}

	public static T GetWeightObject<T>(IList<WeightNode<T>> list)
	{
		return BaseSerch(list).value;
	}

	public static T GetWeightObject<T>(params WeightNode<T>[] array)
	{
		return BaseSerch(array).value;
	}

	public static T GetWeightObject<T>(IList<T> list) where T : IHasWeight
	{
		return BaseSerch(list);
	}

	public static T GetWeightObject<T>(params T[] array) where T : IHasWeight
	{
		return BaseSerch(array);
	}

	private static T BaseSerch<T>(IList<T> list) where T : IHasWeight
	{
		float maxInclusive = list.Sum((T x) => x.Weight);
		float num = UnityEngine.Random.Range(0f, maxInclusive);
		float num2 = 0f;
		for (int i = 0; i < list.Count; i++)
		{
			num2 += list[i].Weight;
			if (num < num2)
			{
				return list[i];
			}
		}
		throw new Exception();
	}
}
