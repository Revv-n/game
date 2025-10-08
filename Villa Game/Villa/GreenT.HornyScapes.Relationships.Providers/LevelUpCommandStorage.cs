using System.Linq;
using GreenT.HornyScapes.Relationships.Models;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Relationships.Providers;

public class LevelUpCommandStorage : SimpleManager<LevelUpCommand>
{
	public override void Add(LevelUpCommand command)
	{
		if (!TryReplaceCommand(command))
		{
			base.Add(command);
		}
	}

	public bool TryGet(Relationship relationship, out LevelUpCommand command)
	{
		command = collection.FirstOrDefault((LevelUpCommand x) => x.Relationship == relationship);
		if (command == LevelUpCommand.Default)
		{
			return false;
		}
		collection.Remove(command);
		return true;
	}

	private bool TryReplaceCommand(LevelUpCommand command)
	{
		LevelUpCommand levelUpCommand = collection.FirstOrDefault((LevelUpCommand other) => other.Relationship == command.Relationship);
		if (levelUpCommand == LevelUpCommand.Default)
		{
			return false;
		}
		int index = collection.IndexOf(levelUpCommand);
		command.Index = levelUpCommand.Index;
		collection[index] = command;
		onNew.OnNext(command);
		return true;
	}
}
