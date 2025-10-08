using System;
using System.Collections.Generic;
using UnityEngine;

namespace Merge.Meta.RoomObjects;

[Serializable]
public class RoomData
{
	[SerializeField]
	public int ID { get; private set; }

	[SerializeField]
	public List<RoomStateData> AllObjects { get; set; } = new List<RoomStateData>();


	public RoomData(int roomID)
	{
		ID = roomID;
	}
}
