using System.Collections.Generic;
using UnityEngine;

namespace GreenT.HornyScapes.Meta.RoomObjects;

[CreateAssetMenu(fileName = "NewObjectConfig", menuName = "DL/Configs/Meta/ObjectConfig")]
public class RoomObjectConfig : BaseObjectConfig
{
	[SerializeField]
	private List<RoomObjectViewInfo> views = new List<RoomObjectViewInfo>();

	[SerializeField]
	private List<RoomObjectReference> roomObjectReferences = new List<RoomObjectReference>();

	public List<RoomObjectViewInfo> Views
	{
		get
		{
			return views;
		}
		set
		{
			views = value;
		}
	}

	public List<RoomObjectReference> RoomObjectReferences => roomObjectReferences;
}
