using System.Collections.Generic;
using Merge.Meta.RoomObjects;
using UnityEngine;

namespace Merge.Meta;

[CreateAssetMenu(fileName = "RoomObjectsBigConfig", menuName = "DL/Configs/Meta/BigRoomObjects")]
public class RoomObjectsBigConfig : ScriptableObject
{
	[SerializeField]
	private List<RoomConfig> roomConfigs;

	public List<RoomConfig> AllRooms => roomConfigs;

	public RoomConfig GetRoomConfig(int room)
	{
		return roomConfigs[room];
	}
}
