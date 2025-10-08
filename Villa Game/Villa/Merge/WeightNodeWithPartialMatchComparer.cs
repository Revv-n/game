using System.Collections.Generic;

namespace Merge;

public class WeightNodeWithPartialMatchComparer : IEqualityComparer<WeightNode<GIData>>
{
	public bool Equals(WeightNode<GIData> x, WeightNode<GIData> y)
	{
		if (x.value.Key.Equals(y.value.Key))
		{
			return x.Weight >= y.Weight;
		}
		return false;
	}

	public int GetHashCode(WeightNode<GIData> obj)
	{
		return obj.value.Key.GetHashCode();
	}
}
