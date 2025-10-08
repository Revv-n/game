using Merge.Meta.RoomObjects;
using Zenject;

namespace GreenT.HornyScapes.Meta.RoomObjects;

public class SpineObjectFactory : BaseRoomObjectFactory<CharacterObjectConfig, CharacterObject>
{
	public SpineObjectFactory(DiContainer container, CharacterObject prefab)
		: base(container, prefab)
	{
	}
}
