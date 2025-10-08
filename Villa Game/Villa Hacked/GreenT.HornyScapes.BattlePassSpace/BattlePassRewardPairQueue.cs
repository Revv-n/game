using System.Collections.Generic;
using GreenT.HornyScapes.BattlePassSpace.RewardCards;

namespace GreenT.HornyScapes.BattlePassSpace;

public class BattlePassRewardPairQueue
{
	private readonly List<BattlePassRewardPairData> _cases;

	public IReadOnlyList<BattlePassRewardPairData> Cases => _cases;

	public BattlePassRewardPairQueue(List<BattlePassRewardPairData> cases)
	{
		_cases = cases;
	}
}
