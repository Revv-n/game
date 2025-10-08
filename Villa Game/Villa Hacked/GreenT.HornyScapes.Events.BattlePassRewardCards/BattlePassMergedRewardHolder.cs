using System.Linq;
using GreenT.HornyScapes.BattlePassSpace.RewardCards;
using UnityEngine;

namespace GreenT.HornyScapes.Events.BattlePassRewardCards;

public class BattlePassMergedRewardHolder : BaseBattlePassRewardHolder
{
	[SerializeField]
	private Transform _container;

	[SerializeField]
	private BattlePassRewardCard _currentCard;

	public override void Set(BattlePassRewardPairData pairCase)
	{
		if (_currentCard == null)
		{
			_currentCard = cardFactory.Create(BattlePassRewardCardFactory.CardViewType.Free);
		}
		_currentCard.transform.SetParent(_container);
		_currentCard.Set(pairCase.FreeReward.First());
		_currentCard.SetBackground(bundleData.MergedRewardHolder);
	}
}
