using GreenT.HornyScapes.BattlePassSpace.Data;

namespace GreenT.HornyScapes.BattlePassSpace;

public class BattlePassDataCase
{
	public BattlePassStartData StartData { get; private set; }

	public BattlePasLevelInfoCase LevelInfo { get; private set; }

	public BattlePassRewardPairQueue RewardPairQueue { get; private set; }

	public BattlePassMergedCurrencyDataCase MergedCurrencyData { get; private set; }

	public void Initialization(BattlePassDataCreateCase createCase)
	{
		LevelInfo = createCase.BattlePasLevelInfoCase;
		StartData = createCase.BattlePassStartData;
		RewardPairQueue = createCase.RewardPairQueue;
		MergedCurrencyData = createCase.MergedCurrencyDataCase;
	}
}
