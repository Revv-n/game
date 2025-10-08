using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace.RewardCards;
using UnityEngine;

namespace GreenT.HornyScapes.Events.BattlePassRewardCards;

public class BattlePassSimpleRewardHolder : BaseBattlePassRewardHolder
{
	[SerializeField]
	private Transform _freeLayoutHolder;

	[SerializeField]
	private Transform _premiumLayoutHolder;

	private readonly List<BattlePassRewardCard> _currentCards = new List<BattlePassRewardCard>();

	public override void Set(BattlePassRewardPairData pairData)
	{
		foreach (BattlePassRewardCard currentCard in _currentCards)
		{
			currentCard.Display(display: false);
		}
		foreach (RewardWithManyConditions item in pairData.FreeReward)
		{
			BattlePassRewardCard view = GetView(BattlePassRewardCardFactory.CardViewType.Free);
			view.transform.SetParent(_freeLayoutHolder);
			view.transform.localScale = Vector3.one;
			view.Set(item);
			_currentCards.Add(view);
		}
		foreach (RewardWithManyConditions item2 in pairData.PremiumReward)
		{
			BattlePassRewardCard view2 = GetView(BattlePassRewardCardFactory.CardViewType.Premium);
			view2.transform.SetParent(_premiumLayoutHolder);
			view2.transform.localScale = Vector3.one;
			view2.Set(item2);
			_currentCards.Add(view2);
		}
	}

	private void OnDestroy()
	{
		foreach (BattlePassRewardCard currentCard in _currentCards)
		{
			Object.Destroy(currentCard.gameObject);
		}
		_currentCards.Clear();
	}

	private BattlePassRewardCard GetView(BattlePassRewardCardFactory.CardViewType type)
	{
		BattlePassRewardCard battlePassRewardCard = _currentCards.FirstOrDefault((BattlePassRewardCard _view) => !_view.IsActive());
		if (battlePassRewardCard == null)
		{
			battlePassRewardCard = cardFactory.Create(type);
			_currentCards.Add(battlePassRewardCard);
		}
		else
		{
			battlePassRewardCard.Display(display: true);
		}
		return battlePassRewardCard;
	}
}
