using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Characters.Skins.Events;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Meta.Decorations;
using StripClub.Model;
using StripClub.UI;

namespace GreenT.HornyScapes.Events;

public class EventRewardCardManager : ViewManager<EventReward, EventRewardCard>
{
	public IReadOnlyList<EventRewardCard> Views => views;

	public override EventRewardCard Display(EventReward source)
	{
		EventRewardCard eventRewardCard = base.Display(source);
		int siblingIndex = views.Sum((EventRewardCard _view) => _view.IsActive() ? 1 : 0) - 1;
		eventRewardCard.SiblingIndex = siblingIndex;
		return eventRewardCard;
	}

	protected override bool CheckAvailableView(EventRewardCard view, EventReward source)
	{
		bool num = base.CheckAvailableView(view, source);
		bool flag = CheckSetCorrectContent(view, source);
		return num && flag;
	}

	private bool CheckSetCorrectContent(EventRewardCard view, EventReward source)
	{
		if (!(view is EventSkinRewardCard))
		{
			if (!(view is EventGirlRewardCard))
			{
				if (view is EventItemsRewardCard)
				{
					LinkedContent content = source.Content;
					return content is CurrencyLinkedContent || content is MergeItemLinkedContent || content is DecorationLinkedContent || content is LootboxLinkedContent || content is BattlePassLevelLinkedContent;
				}
				throw new ArgumentOutOfRangeException("view");
			}
			return source.Content is CardLinkedContent;
		}
		return source.Content is SkinLinkedContent;
	}
}
