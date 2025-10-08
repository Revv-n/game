using System.Collections.Generic;
using GreenT.HornyScapes.Meta.RoomObjects;
using UnityEngine;

namespace Merge.Meta.RoomObjects;

[CreateAssetMenu(fileName = "RoomConfig", menuName = "DL/Configs/Meta/Room")]
public class RoomConfig : ScriptableObject
{
	[SerializeField]
	private int roomID;

	[SerializeField]
	private Vector2 position;

	[SerializeField]
	private List<BaseObjectConfig> configs = new List<BaseObjectConfig>();

	public int RoomID => roomID;

	public Vector2 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
		}
	}

	public List<BaseObjectConfig> ObjectConfigs => configs;
}
