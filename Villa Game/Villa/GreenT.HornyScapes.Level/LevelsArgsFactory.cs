using Zenject;

namespace GreenT.HornyScapes.Level;

public class LevelsArgsFactory : IFactory<LevelsArgsMapper, LevelsArgs>, IFactory
{
	public LevelsArgs Create(LevelsArgsMapper param)
	{
		return new LevelsArgs(param.level, param.xp_delta, param.xp_delta, param.chest_id);
	}
}
