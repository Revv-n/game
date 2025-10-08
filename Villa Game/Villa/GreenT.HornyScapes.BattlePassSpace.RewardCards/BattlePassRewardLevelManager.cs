using GreenT.HornyScapes.Events.BattlePassRewardCards;
using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.BattlePassSpace.RewardCards;

public class BattlePassRewardLevelManager : ViewManager<BattlePassRewardPairData, BattlePassRewardLevelView>
{
	public void DestroyAllViews()
	{
		while (views.Count > 0)
		{
			BattlePassRewardLevelView battlePassRewardLevelView = views[0];
			views.Remove(battlePassRewardLevelView);
			Object.Destroy(battlePassRewardLevelView.gameObject);
		}
	}
}
