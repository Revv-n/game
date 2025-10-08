using System.Linq;
using GreenT.Model.Collections;
using Merge.Meta.RoomObjects;

namespace GreenT.HornyScapes.Meta;

public class RoomConfigManager : SimpleManager<RoomConfig>
{
	public void RemoveByIDs(params int[] roomIDs)
	{
		foreach (int id in roomIDs)
		{
			RoomConfig roomConfig = collection.FirstOrDefault((RoomConfig _roomConfig) => _roomConfig.RoomID == id);
			if (roomConfig != null)
			{
				collection.Remove(roomConfig);
			}
		}
	}
}
