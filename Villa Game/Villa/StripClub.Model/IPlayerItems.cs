using System;
using System.Collections.Generic;
using GreenT.Data;

namespace StripClub.Model;

public interface IPlayerItems : ISavableState
{
	event EventHandler OnUpdate;

	bool Contains(Guid guid);

	Item GetItem(Guid guid);

	IEnumerable<Item> GetItems<T>() where T : IItemInfo;

	IEnumerable<T> GetItemInfos<T>() where T : IItemInfo;

	void AddItem<T>(T newItem) where T : Item;

	bool TryToSpend(IEnumerable<ItemLot> items);
}
