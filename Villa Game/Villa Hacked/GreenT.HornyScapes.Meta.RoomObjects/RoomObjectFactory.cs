using Merge.Meta.RoomObjects;
using Zenject;

namespace GreenT.HornyScapes.Meta.RoomObjects;

public class RoomObjectFactory : BaseRoomObjectFactory<RoomObjectConfig, RoomObject>
{
	public RoomObjectFactory(DiContainer container, RoomObject prefab)
		: base(container, prefab)
	{
	}
}
