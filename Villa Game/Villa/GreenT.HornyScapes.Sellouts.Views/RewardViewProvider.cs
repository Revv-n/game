using System.Collections.Generic;
using GreenT.HornyScapes.Sellouts.Models;
using UnityEngine;

namespace GreenT.HornyScapes.Sellouts.Views;

public class RewardViewProvider : MonoBehaviour
{
	[SerializeField]
	private SelloutRewardViewProvider _selloutRewardViewProvider;

	public List<SelloutRewardView> VisibleViews = new List<SelloutRewardView>();

	public SelloutRewardView Display(int id, SelloutRewardsInfo rewardInfos)
	{
		SelloutRewardView selloutRewardView = _selloutRewardViewProvider.Display((id, rewardInfos));
		VisibleViews.Add(selloutRewardView);
		return selloutRewardView;
	}

	public void HideAll()
	{
		_selloutRewardViewProvider.HideAll();
		VisibleViews.Clear();
	}
}
