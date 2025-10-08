using System;
using GreenT.Types;
using UnityEngine;

namespace GreenT.HornyScapes.Meta.RoomObjects;

[Serializable]
public struct RoomObjectReference : ISerializationCallbackReceiver
{
	public int RoomId;

	public int ObjectId;

	public ReferenceObjectNextState NextState;

	public CompositeIdentificator ID;

	public RoomObjectReference(int roomID, int objectID, ReferenceObjectNextState nextState)
	{
		RoomId = roomID;
		ObjectId = objectID;
		NextState = nextState;
		ID = new CompositeIdentificator(RoomId, ObjectId);
		CompositeIdentificator iD = ID;
		Debug.LogError("Room ID: " + iD.ToString());
	}

	public void OnAfterDeserialize()
	{
		ID = new CompositeIdentificator(RoomId, ObjectId);
	}

	public void OnBeforeSerialize()
	{
	}
}
