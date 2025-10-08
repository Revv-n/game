using GreenT.HornyScapes.Relationships.Models;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Factories;

public class LevelUpCommandFactory : IFactory<Relationship, int, LevelUpCommand>, IFactory
{
	public LevelUpCommand Create(Relationship relationship, int index)
	{
		return new LevelUpCommand(relationship, index);
	}
}
