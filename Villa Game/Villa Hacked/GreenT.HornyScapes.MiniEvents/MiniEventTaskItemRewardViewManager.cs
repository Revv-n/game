using System;
using System.Linq;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Meta.Decorations;
using StripClub.Model;
using StripClub.UI;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventTaskItemRewardViewManager : ViewManager<LinkedContent, MiniEventTaskRewardItemView>
{
	public override MiniEventTaskRewardItemView Display(LinkedContent source)
	{
		MiniEventTaskRewardItemView miniEventTaskRewardItemView = base.Display(source);
		int siblingIndex = views.Sum((MiniEventTaskRewardItemView _view) => _view.IsActive() ? 1 : 0) - 1;
		miniEventTaskRewardItemView.SiblingIndex = siblingIndex;
		return miniEventTaskRewardItemView;
	}

	protected override bool CheckAvailableView(MiniEventTaskRewardItemView view, LinkedContent source)
	{
		if (base.CheckAvailableView(view, source))
		{
			return CheckSetCorrectContent(view, source);
		}
		return false;
	}

	private bool CheckSetCorrectContent(MiniEventTaskRewardItemView view, LinkedContent source)
	{
		if (!(view is MiniEventTaskLootboxRewardItemView))
		{
			if (!(view is MiniEventTaskCharactersRewardItemView))
			{
				if (!(view is MiniEventTaskMergeRewardItemView))
				{
					if (!(view is MiniEventTaskDecorationRewardItemView))
					{
						if (view is MiniEventTaskCurrencyRewardItemView)
						{
							return source is CurrencyLinkedContent;
						}
						throw new ArgumentOutOfRangeException("view");
					}
					return source is DecorationLinkedContent;
				}
				return source is MergeItemLinkedContent;
			}
			return source is CardLinkedContent || source is SkinLinkedContent;
		}
		return source is LootboxLinkedContent;
	}
}
