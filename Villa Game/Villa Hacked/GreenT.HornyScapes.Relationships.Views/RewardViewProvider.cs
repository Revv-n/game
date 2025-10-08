using System.Collections.Generic;
using GreenT.HornyScapes.Dates.Models;
using GreenT.HornyScapes.Events;
using StripClub.Model;
using UnityEngine;

namespace GreenT.HornyScapes.Relationships.Views;

public class RewardViewProvider : MonoBehaviour
{
	[SerializeField]
	private LootboxRewardViewProvider _lootboxRewardViewProvider;

	[SerializeField]
	private DateRewardViewProvider _dateRewardViewProvider;

	[SerializeField]
	private ComingSoonDateRewardViewProvider _comingSoonDateRewardViewProvider;

	public List<BaseRewardView> VisibleViews = new List<BaseRewardView>();

	public BaseRewardView Display(int id, IReadOnlyList<RewardWithManyConditions> rewards)
	{
		LinkedContent content = rewards[0].Content;
		BaseRewardView baseRewardView = ((content is DateLinkedContent) ? _dateRewardViewProvider.Display((id, rewards)) : ((!(content is ComingSoonDateLinkedContent)) ? ((BaseRewardView)_lootboxRewardViewProvider.Display((id, rewards))) : ((BaseRewardView)_comingSoonDateRewardViewProvider.Display((id, rewards)))));
		BaseRewardView baseRewardView2 = baseRewardView;
		VisibleViews.Add(baseRewardView2);
		return baseRewardView2;
	}

	public void HideAll()
	{
		_dateRewardViewProvider.HideAll();
		_lootboxRewardViewProvider.HideAll();
		VisibleViews.Clear();
	}
}
