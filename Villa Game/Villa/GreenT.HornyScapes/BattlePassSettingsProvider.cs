using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes;

public sealed class BattlePassSettingsProvider : SimpleManager<BattlePass>
{
	public BattlePass GetBattlePass(int id)
	{
		return Collection.FirstOrDefault((BattlePass x) => x.ID == id);
	}

	public bool TryGetBattlePass(int id, out BattlePass battlePass)
	{
		battlePass = GetBattlePass(id);
		return battlePass != null;
	}

	public void RemoveBattlePass(BattlePass battlePass)
	{
		collection.Remove(battlePass);
	}
}
