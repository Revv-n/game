using System;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Meta.Decorations;
using StripClub.Model;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class RoulettePreviewBigCardManager : RoulettePreviewCardManager
{
	public RoulettePreviewBigCardManager()
	{
		_isBig = true;
	}

	protected override bool CheckSetCorrectContent(PromoCardView view, LinkedContent source)
	{
		if (!(view is BigAnyPromoCardView))
		{
			if (view is BigGirlPromoCardView)
			{
				return source is CardLinkedContent || source is SkinLinkedContent;
			}
			throw new ArgumentOutOfRangeException("view");
		}
		return source is CurrencyLinkedContent || source is MergeItemLinkedContent || source is DecorationLinkedContent || source is LootboxLinkedContent || source is BattlePassLevelLinkedContent;
	}
}
