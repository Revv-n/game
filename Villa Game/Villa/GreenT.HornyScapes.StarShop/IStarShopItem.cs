using System;
using GreenT.Types;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using StripClub.Model.Data;

namespace GreenT.HornyScapes.StarShop;

public interface IStarShopItem
{
	int ID { get; }

	int LootboxIdReward { get; }

	Cost Cost { get; }

	CompositeLocker Lock { get; }

	EntityStatus State { get; }

	CompositeIdentificator HouseObjectID { get; }

	IObservable<StarShopItem> OnUpdate { get; }

	void SetState(EntityStatus state);
}
