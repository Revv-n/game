using GreenT.HornyScapes.StarShop;
using Merge.Meta.RoomObjects;
using StripClub.Model;

namespace GreenT.HornyScapes;

public class StepLocker : Locker
{
	public int ItemID { get; }

	public EntityStatus State { get; }

	public StepLocker(int itemID, EntityStatus state)
	{
		ItemID = itemID;
		State = state;
	}

	public void Set(IStarShopItem item)
	{
		if (item.ID == ItemID)
		{
			isOpen.Value = (State & item.State) == item.State;
		}
	}
}
