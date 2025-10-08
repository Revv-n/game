using Merge.Meta.RoomObjects;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Meta.RoomObjects;

public class RoomObjectFactoryAggregator : IFactory<BaseObjectConfig, Transform, IGameRoomObject<BaseObjectConfig>>, IFactory
{
	private readonly IFactory<CharacterObjectConfig, Transform, CharacterObject> spineObjectFactory;

	private readonly IFactory<RoomObjectConfig, Transform, RoomObject> roomObjectFactory;

	private readonly IFactory<AnimatedObjectConfig, Transform, AnimatedRoomObject> animatedRoomObjectFactory;

	public RoomObjectFactoryAggregator(IFactory<CharacterObjectConfig, Transform, CharacterObject> spineObjectFactory, IFactory<RoomObjectConfig, Transform, RoomObject> roomObjectFactory, IFactory<AnimatedObjectConfig, Transform, AnimatedRoomObject> animatedRoomObjectFactory)
	{
		this.spineObjectFactory = spineObjectFactory;
		this.roomObjectFactory = roomObjectFactory;
		this.animatedRoomObjectFactory = animatedRoomObjectFactory;
	}

	public IGameRoomObject<BaseObjectConfig> Create(BaseObjectConfig config, Transform parent)
	{
		IGameRoomObject<BaseObjectConfig> result = null;
		if (!(config is RoomObjectConfig param))
		{
			if (!(config is CharacterObjectConfig param2))
			{
				if (config is AnimatedObjectConfig param3)
				{
					result = animatedRoomObjectFactory.Create(param3, parent);
				}
			}
			else
			{
				result = spineObjectFactory.Create(param2, parent);
			}
		}
		else
		{
			result = roomObjectFactory.Create(param, parent);
		}
		return result;
	}
}
