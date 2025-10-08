using System;
using StripClub.Model;
using UnityEngine;

namespace GreenT.HornyScapes.Meta.Data;

[Serializable]
public class RoomDataConfig
{
	[field: SerializeField]
	public int RoomId { get; private set; }

	[field: SerializeField]
	public UnlockType UnlockType { get; private set; } = UnlockType.StepComplete;


	[field: SerializeField]
	public string UnlockValue { get; private set; } = "0";


	[field: SerializeField]
	public bool IsPreload { get; private set; }
}
