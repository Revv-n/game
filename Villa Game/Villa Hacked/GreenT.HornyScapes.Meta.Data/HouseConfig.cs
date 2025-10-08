using UnityEngine;

namespace GreenT.HornyScapes.Meta.Data;

[CreateAssetMenu(fileName = "House", menuName = "DL/Configs/Meta/House", order = 2)]
public class HouseConfig : ScriptableObject
{
	[field: SerializeField]
	public RoomDataConfig[] RoomDatas { get; private set; }
}
