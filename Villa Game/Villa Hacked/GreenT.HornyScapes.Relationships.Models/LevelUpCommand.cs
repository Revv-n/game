using System;
using GreenT.HornyScapes.Relationships.Animations;

namespace GreenT.HornyScapes.Relationships.Models;

public struct LevelUpCommand : IEquatable<LevelUpCommand>
{
	public int Index;

	public Relationship Relationship { get; }

	public static LevelUpCommand Default { get; } = new LevelUpCommand(null, 0);


	public LevelUpCommand(Relationship relationship, int index)
	{
		Relationship = relationship;
		Index = index;
	}

	public void Execute(LevelUpAnimationService levelUpAnimationService)
	{
		levelUpAnimationService.PlayAnimation(Relationship.ID, Index);
	}

	public static bool operator ==(LevelUpCommand command, LevelUpCommand otherCommand)
	{
		return command.Equals(otherCommand);
	}

	public static bool operator !=(LevelUpCommand command, LevelUpCommand otherCommand)
	{
		return !command.Equals(otherCommand);
	}

	public bool Equals(LevelUpCommand other)
	{
		if (Relationship == other.Relationship)
		{
			return Index == other.Index;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is LevelUpCommand other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine<Relationship, int>(Relationship, Index);
	}
}
