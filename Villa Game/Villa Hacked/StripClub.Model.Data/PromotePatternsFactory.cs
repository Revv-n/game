using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Cards;
using Zenject;

namespace StripClub.Model.Data;

internal class PromotePatternsFactory : IFactory<IEnumerable<PromotePatternMapper>, PromotePatterns>, IFactory
{
	public PromotePatterns Create(IEnumerable<PromotePatternMapper> param)
	{
		IEnumerable<IGrouping<int, PromotePatternMapper>> enumerable = from _data in param
			group _data by _data.id;
		PromotePatterns promotePatterns = new PromotePatterns();
		foreach (IGrouping<int, PromotePatternMapper> item in enumerable)
		{
			Pattern pattern = new Pattern();
			Rarity[] array = (Rarity[])Enum.GetValues(typeof(Rarity));
			foreach (Rarity key in array)
			{
				pattern[key] = new Dictionary<int, PromotePattern>();
			}
			foreach (PromotePatternMapper item2 in item)
			{
				pattern[item2.rarity][item2.current_level] = new PromotePattern(item2.promote_cards_value, item2.promote_resource_value);
			}
			Debug(pattern);
			promotePatterns[item.Key] = pattern;
		}
		return promotePatterns;
	}

	private void Debug(Pattern pattern)
	{
		foreach (Rarity key in pattern.Keys)
		{
			foreach (int key2 in pattern[key].Keys)
			{
				_ = key2;
			}
		}
	}
}
