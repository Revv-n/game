using System.Collections.Generic;
using GreenT.HornyScapes.Meta.RoomObjects;
using Merge.Meta.RoomObjects;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Meta;

public class RoomFactory : IFactory<RoomConfig, Transform, Room>, IFactory
{
	private readonly IFactory<BaseObjectConfig, Transform, IGameRoomObject<BaseObjectConfig>> roomObjectFactory;

	public Transform RoomParent { get; private set; }

	public RoomFactory(IFactory<BaseObjectConfig, Transform, IGameRoomObject<BaseObjectConfig>> roomObjectFactory)
	{
		this.roomObjectFactory = roomObjectFactory;
	}

	public virtual Room Create(RoomConfig roomConfig, Transform parent)
	{
		if (roomConfig == null)
		{
			return new Room(-1, CreateRoomParent(parent, "Empty", Vector2.zero));
		}
		Dictionary<string, IRoomObject<BaseObjectConfig>> dictionary = new Dictionary<string, IRoomObject<BaseObjectConfig>>();
		RoomParent = CreateRoomParent(parent, roomConfig.RoomID.ToString(), roomConfig.Position);
		for (int i = 0; i < roomConfig.ObjectConfigs.Count; i++)
		{
			BaseObjectConfig baseObjectConfig = roomConfig.ObjectConfigs[i];
			IGameRoomObject<BaseObjectConfig> gameRoomObject = roomObjectFactory.Create(baseObjectConfig, RoomParent);
			if (gameRoomObject != null)
			{
				if (dictionary.ContainsKey(baseObjectConfig.ObjectName))
				{
					Debug.LogError("Room Object with name: " + baseObjectConfig.ObjectName + " has been added already. Check you Room Config with ID: " + roomConfig.RoomID);
				}
				else
				{
					dictionary.Add(baseObjectConfig.ObjectName, gameRoomObject);
				}
			}
		}
		return new Room(roomConfig.RoomID, RoomParent, dictionary);
	}

	private static Transform CreateRoomParent(Transform parent, string roomId, Vector2 position)
	{
		Transform transform = new GameObject("Room_" + roomId).transform;
		transform.parent = parent;
		transform.localScale = Vector3.one;
		transform.position = position;
		return transform;
	}
}
