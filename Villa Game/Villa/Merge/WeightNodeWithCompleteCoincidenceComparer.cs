using System.Collections.Generic;

namespace Merge;

public class WeightNodeWithCompleteCoincidenceComparer : IEqualityComparer<WeightNode<GIData>>
{
	public bool Equals(WeightNode<GIData> x, WeightNode<GIData> y)
	{
		if (x.value.Key.Equals(y.value.Key))
		{
			return (int)x.Weight == (int)y.Weight;
		}
		return false;
	}

	public int GetHashCode(WeightNode<GIData> obj)
	{
		return obj.value.Key.GetHashCode();
	}
}
