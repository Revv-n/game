using System;
using System.Collections.Generic;

namespace StripClub.Model.Data;

public interface IItemBase
{
	void Add(IItemInfo info);

	IItemInfo GetItemInfo(Guid guid);

	IItemInfo GetItemInfoByName(string name);

	IEnumerable<IItemInfo> GetItemsInfo(params Type[] types);
}
