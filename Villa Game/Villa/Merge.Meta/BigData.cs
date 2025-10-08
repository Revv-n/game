using System;
using System.Collections.Generic;
using Merge.Meta.RoomObjects;
using UnityEngine;

namespace Merge.Meta;

[Serializable]
public class BigData : Data
{
	[SerializeField]
	public int ActiveRoomID { get; set; }

	[SerializeField]
	public List<RoomData> AllRooms { get; set; } = new List<RoomData>();


	public RoomData GetActiveRoom()
	{
		return AllRooms[ActiveRoomID];
	}
}
