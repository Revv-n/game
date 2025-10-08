using GreenT.HornyScapes.BattlePassSpace.RewardCards;
using StripClub.UI;
using Zenject;

namespace GreenT.HornyScapes.Events.BattlePassRewardCards;

public abstract class BaseBattlePassRewardHolder : MonoView<BattlePassRewardPairData>
{
	protected BattlePassBundleData bundleData;

	protected BattlePassRewardCardFactory cardFactory;

	[Inject]
	private void Construct(BattlePassRewardCardFactory cardFactory)
	{
		this.cardFactory = cardFactory;
	}

	public void SetBundle(BattlePassBundleData bundleData)
	{
		this.bundleData = bundleData;
	}

	public abstract override void Set(BattlePassRewardPairData pair);
}
