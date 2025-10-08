using System;
using GreenT.HornyScapes.Data;
using StripClub.Model;

namespace GreenT.HornyScapes.Meta.Decorations;

[Serializable]
[Mapper]
public class DecorationMapper
{
	public int id;

	public UnlockType[] unlock_type;

	public string[] unlock_value;

	public int room_id;

	public int object_number;
}
