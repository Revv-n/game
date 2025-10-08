using System;
using UnityEngine;

namespace Merge.Meta.RoomObjects;

[Serializable]
public class RoomStateData
{
	[SerializeField]
	public string RoomObjectName { get; set; }

	[field: SerializeField]
	public EntityStatus Status { get; set; }

	[field: SerializeField]
	public int SelectedSkin { get; set; }

	public bool IsBlocked => Status == EntityStatus.Blocked;

	public bool IsInProgress => Status == EntityStatus.InProgress;

	public bool IsRewarded => Status == EntityStatus.Rewarded;

	public bool Complete => Status == EntityStatus.Complete;

	public RoomStateData(string name)
	{
		RoomObjectName = name;
		Initialize();
	}

	public void Initialize()
	{
		Status = EntityStatus.Blocked;
		SelectedSkin = 0;
	}
}
