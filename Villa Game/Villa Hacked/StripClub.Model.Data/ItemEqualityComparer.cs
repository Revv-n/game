using System.Collections.Generic;

namespace StripClub.Model.Data;

public class ItemEqualityComparer : IEqualityComparer<IItemInfo>
{
	public bool Equals(IItemInfo x, IItemInfo y)
	{
		return x == y;
	}

	public int GetHashCode(IItemInfo obj)
	{
		return obj.GetHashCode();
	}
}
