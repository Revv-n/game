using Merge.Meta.RoomObjects;

namespace GreenT.HornyScapes.Events;

public static class EnumConverter
{
	public static int ConvertToInt(this EntityStatus state)
	{
		return state switch
		{
			EntityStatus.InProgress => 1, 
			EntityStatus.Complete => 2, 
			EntityStatus.Rewarded => 3, 
			_ => 0, 
		};
	}
}
