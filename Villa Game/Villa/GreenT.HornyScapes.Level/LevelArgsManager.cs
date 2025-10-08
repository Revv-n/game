using System.Linq;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Level;

public class LevelArgsManager : SimpleManager<LevelsArgs>
{
	public LevelsArgs GetArgs(int levelId)
	{
		return Collection.First((LevelsArgs arg) => arg.Level == levelId);
	}

	public bool HasArgs(int levelId)
	{
		return Collection.Any((LevelsArgs arg) => arg.Level == levelId);
	}
}
