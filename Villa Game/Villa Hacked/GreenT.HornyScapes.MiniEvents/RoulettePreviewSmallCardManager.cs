using System;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Meta.Decorations;
using StripClub.Model;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class RoulettePreviewSmallCardManager : RoulettePreviewCardManager
{
	public RoulettePreviewSmallCardManager()
	{
		_isBig = false;
	}

	protected override bool CheckSetCorrectContent(PromoCardView view, LinkedContent source)
	{
		if (!(view is SmallAnyPromoCardView))
		{
			if (view is SmallGirlPromoCardView)
			{
				return source is CardLinkedContent || source is SkinLinkedContent;
			}
			throw new ArgumentOutOfRangeException("view");
		}
		return source is CurrencyLinkedContent || source is MergeItemLinkedContent || source is DecorationLinkedContent || source is LootboxLinkedContent || source is BattlePassLevelLinkedContent;
	}
}
