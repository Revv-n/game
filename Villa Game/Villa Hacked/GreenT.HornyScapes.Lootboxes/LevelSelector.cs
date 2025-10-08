namespace GreenT.HornyScapes.Lootboxes;

public class LevelSelector : Selector
{
	public LevelType Level { get; }

	public LevelSelector(LevelType level)
	{
		Level = level;
	}
}
