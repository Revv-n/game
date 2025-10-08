using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Meta;
using GreenT.HornyScapes.Meta.RoomObjects;
using Merge.Meta.RoomObjects;
using UnityEngine;

namespace Merge.Meta;

public static class RoomFactory
{
	public static Room BuildRoom(RoomObject prefab, Transform parent, RoomConfig roomConfig, Func<RoomObjectConfig, RoomStateData> getDataByConfig)
	{
		Dictionary<string, IRoomObject<BaseObjectConfig>> dictionary = new Dictionary<string, IRoomObject<BaseObjectConfig>>();
		parent.localScale = Vector3.one;
		foreach (RoomObjectConfig item in roomConfig.ObjectConfigs.OfType<RoomObjectConfig>())
		{
			RoomObject roomObject = CreateRO(prefab, parent);
			RoomStateData roomStateData = getDataByConfig(item);
			if (roomStateData == null)
			{
				roomStateData = new RoomStateData(item.name);
			}
			roomObject.Init(roomStateData, item);
			dictionary.Add(item.ObjectName, roomObject);
		}
		parent.localScale = Vector3.one * GetRatio(roomConfig);
		return new Room(roomConfig.RoomID, parent, dictionary);
	}

	private static RoomObject CreateRO(RoomObject prefab, Transform parent)
	{
		RoomObject roomObject = UnityEngine.Object.Instantiate(prefab);
		roomObject.transform.SetParent(parent);
		roomObject.transform.SetDefault();
		return roomObject;
	}

	private static float GetRatio(RoomConfig roomConfig)
	{
		float num = 1.3333f;
		Camera main = Camera.main;
		return ((float)main.pixelWidth / (float)main.pixelHeight + 0.1f) / num;
	}
}
