using GreenT.HornyScapes.BattlePassSpace.RewardCards;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events.BattlePassRewardCards;

public class BattlePassRewardLevelViewFactory : ViewFactory<BattlePassRewardPairData, BattlePassRewardLevelView>
{
	public BattlePassRewardLevelViewFactory(DiContainer diContainer, Transform objectContainer, BattlePassRewardLevelView prefab)
		: base(diContainer, objectContainer, prefab)
	{
	}
}
