using System;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Meta.Decorations;
using StripClub.Model;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes._HornyScapes._Scripts.Cheats;

public static class CheatUtils
{
	public static void TryChangeAmount(this ICurrencyProcessor currencyProcessor, CurrencyType currencyType, int value)
	{
		if (value >= 0)
		{
			currencyProcessor.TryAdd(currencyType, value);
		}
		else
		{
			currencyProcessor.TrySpent(currencyType, -value);
		}
	}

	public static RewType GetRewTypeFromLinked(this LinkedContent content)
	{
		if (!(content is CardLinkedContent))
		{
			if (!(content is BattlePassLevelLinkedContent))
			{
				if (!(content is CurrencyLinkedContent))
				{
					if (!(content is MergeItemLinkedContent))
					{
						if (!(content is LootboxLinkedContent))
						{
							if (!(content is SkinLinkedContent))
							{
								if (content is DecorationLinkedContent)
								{
									return RewType.Decorations;
								}
								throw new NotImplementedException("There is no behaviour for this content:" + content.Type.Name);
							}
							return RewType.Skin;
						}
						return RewType.Lootbox;
					}
					return RewType.MergeItem;
				}
				return RewType.Resource;
			}
			return RewType.Level;
		}
		return RewType.Characters;
	}

	public static string GetIDFromSelector(this Selector selector)
	{
		if (!(selector is CurrencySelector { Currency: var currency }))
		{
			if (!(selector is LevelSelector { Level: var level }))
			{
				if (!(selector is CardSelector { Rarity: var rarity } cardSelector))
				{
					if (selector is SelectorByID { ID: var iD })
					{
						return iD.ToString();
					}
					throw new NotImplementedException("There is no behaviour for this content:" + selector.GetType().Name);
				}
				return rarity.ToString() + " - " + cardSelector.Pool;
			}
			return level.ToString();
		}
		return currency.ToString();
	}
}
