using System;
using GreenT.Data;

namespace GreenT.HornyScapes;

[Serializable]
public class PlayerExpMemento : Memento
{
	public int Progress { get; }

	public int Level { get; }

	public PlayerExpMemento(PlayerExperience playerExp)
		: base(null)
	{
		Progress = playerExp.XP.Value;
		Level = playerExp.Level.Value;
	}
}
