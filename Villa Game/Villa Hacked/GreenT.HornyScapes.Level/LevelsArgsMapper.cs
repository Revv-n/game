using GreenT.HornyScapes.Data;

namespace GreenT.HornyScapes.Level;

[Mapper]
public class LevelsArgsMapper
{
	public int level;

	public int xp_delta;

	public int xp_total;

	public int chest_id;

	public LevelsArgsMapper(int level, int xp_delta, int xp_total, int chest_id)
	{
		this.level = level;
		this.xp_delta = xp_delta;
		this.xp_total = xp_total;
		this.chest_id = chest_id;
	}
}
