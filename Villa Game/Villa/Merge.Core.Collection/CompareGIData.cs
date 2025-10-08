using System.Collections.Generic;

namespace Merge.Core.Collection;

public class CompareGIData : IComparer<GIData>
{
	public int Compare(GIData x, GIData y)
	{
		int hashCode = x.Key.Collection.GetHashCode();
		int hashCode2 = y.Key.Collection.GetHashCode();
		if (hashCode > hashCode2)
		{
			return 1;
		}
		if (hashCode == hashCode2)
		{
			if (x.Key.ID > y.Key.ID)
			{
				return 1;
			}
			if (x.Key.ID == y.Key.ID)
			{
				return 0;
			}
		}
		return -1;
	}
}
