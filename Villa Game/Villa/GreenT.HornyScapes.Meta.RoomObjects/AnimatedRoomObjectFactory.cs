using Merge.Meta.RoomObjects;
using Zenject;

namespace GreenT.HornyScapes.Meta.RoomObjects;

public class AnimatedRoomObjectFactory : BaseRoomObjectFactory<AnimatedObjectConfig, AnimatedRoomObject>
{
	public AnimatedRoomObjectFactory(DiContainer container, AnimatedRoomObject prefab)
		: base(container, prefab)
	{
	}
}
