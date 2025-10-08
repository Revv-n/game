using System.Collections.Generic;
using System.Linq;
using StripClub.Model;

namespace GreenT.HornyScapes.Sellouts.Views;

public static class LootboxContentExtensions
{
	public static List<LinkedContent> GetInnerContent(IEnumerable<LinkedContent> linkedContents)
	{
		List<LinkedContent> list = new List<LinkedContent>();
		IEnumerable<LootboxLinkedContent> enumerable = linkedContents.OfType<LootboxLinkedContent>();
		foreach (LootboxLinkedContent item in enumerable)
		{
			LinkedContent linkedContent = item.Lootbox.PrepareContent();
			list.Add(linkedContent);
			while (linkedContent.Next() != null)
			{
				linkedContent = linkedContent.Next();
				list.Add(linkedContent);
			}
		}
		list.AddRange(linkedContents.Except(enumerable));
		return list;
	}
}
