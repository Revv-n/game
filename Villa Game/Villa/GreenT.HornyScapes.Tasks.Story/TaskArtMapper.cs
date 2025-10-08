using System;
using StripClub.Model;

namespace GreenT.HornyScapes.Tasks.Story;

[Serializable]
public class TaskArtMapper
{
	public int id;

	public UnlockType[] unlock_type;

	public string[] unlock_value;
}
