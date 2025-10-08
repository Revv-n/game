using System;
using System.Linq;
using GreenT.Model.Collections;
using Merge.Meta.RoomObjects;
using UniRx;

namespace GreenT.HornyScapes.StarShop;

public class StarShopManager : SimpleManager<StarShopItem>
{
	public IObservable<StarShopItem> OnUpdate { get; private set; }

	public StarShopManager()
	{
		OnUpdate = Collection.ToObservable().Merge(onNew).SelectMany((StarShopItem _item) => _item.OnUpdate);
	}

	public StarShopItem GetItem(int id)
	{
		try
		{
			return collection.First((StarShopItem _item) => _item.ID == id);
		}
		catch (InvalidOperationException innerException)
		{
			throw innerException.SendException($"{GetType().Name}: There is no item with id: {id} \n");
		}
	}

	public int GetMaxId()
	{
		return collection.Max((StarShopItem _story) => _story.ID);
	}

	public int GetCompleteMaxId()
	{
		if (!collection.Any((StarShopItem x) => x.State.HasFlag(EntityStatus.Rewarded)))
		{
			return -1;
		}
		return collection.Where((StarShopItem x) => x.State.HasFlag(EntityStatus.Rewarded)).Max((StarShopItem _story) => _story.ID);
	}

	public int GetLastId()
	{
		if (!collection.Any((StarShopItem x) => x.State.HasFlag(EntityStatus.InProgress) || x.State.HasFlag(EntityStatus.Complete)))
		{
			return -1;
		}
		return collection.Where((StarShopItem x) => x.State.HasFlag(EntityStatus.InProgress) || x.State.HasFlag(EntityStatus.Complete)).Max((StarShopItem _story) => _story.ID);
	}

	public StarShopItem GetLast()
	{
		int lastID = GetMaxId();
		return collection.Where((StarShopItem _story) => _story.ID == lastID).First();
	}

	public void Initialize()
	{
		foreach (StarShopItem item in Collection)
		{
			item.Initialize();
		}
	}
}
