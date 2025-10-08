using System;

namespace GreenT.HornyScapes.Meta;

public readonly struct HouseObjectIdentificator : IEquatable<HouseObjectIdentificator>
{
	public int RoomID { get; }

	public int ObjectID { get; }

	public HouseObjectIdentificator(int roomID, int objectID)
	{
		RoomID = roomID;
		ObjectID = objectID;
	}

	public bool Equals(HouseObjectIdentificator other)
	{
		if (RoomID == other.RoomID)
		{
			return ObjectID == other.ObjectID;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is HouseObjectIdentificator other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return RoomID + ObjectID;
	}

	public static bool operator ==(HouseObjectIdentificator x, HouseObjectIdentificator y)
	{
		return x.Equals(y);
	}

	public static bool operator !=(HouseObjectIdentificator x, HouseObjectIdentificator y)
	{
		return !x.Equals(y);
	}
}
