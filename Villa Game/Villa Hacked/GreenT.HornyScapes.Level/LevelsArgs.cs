namespace GreenT.HornyScapes.Level;

public class LevelsArgs
{
	public int Level;

	public int XpDelta;

	public int XpTotal;

	public int ChestId;

	public LevelsArgs(int level, int xp_delta, int xp_total, int chest_id)
	{
		Level = level;
		XpDelta = xp_delta;
		XpTotal = xp_total;
		ChestId = chest_id;
	}
}
