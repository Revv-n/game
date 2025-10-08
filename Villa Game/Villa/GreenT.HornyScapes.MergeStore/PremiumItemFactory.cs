using System;
using System.Collections.Generic;
using System.Linq;
using Merge;

namespace GreenT.HornyScapes.MergeStore;

public class PremiumItemFactory
{
	private readonly ItemFactoryHelper _itemFactoryHelper;

	private readonly MergeItemInfoService _mergeItemInfoService;

	public PremiumItemFactory(ItemFactoryHelper itemFactoryHelper, MergeItemInfoService mergeItemInfoService)
	{
		_itemFactoryHelper = itemFactoryHelper;
		_mergeItemInfoService = mergeItemInfoService;
	}

	public Item[] Generate(StoreSection section, string bundle)
	{
		List<string> allPremiumItemShopSections = _mergeItemInfoService.GetAllPremiumItemShopSections(section.ContentType, bundle);
		if (allPremiumItemShopSections.Count < section.ItemsCount)
		{
			throw new ArgumentException($"Config Items содержит всего {allPremiumItemShopSections.Count} " + $"Premium вариантов из {section.ItemsCount} необходимых {section.ContentType} {bundle}");
		}
		List<Item> list = new List<Item>(section.ItemsCount);
		foreach (string item2 in allPremiumItemShopSections.OrderBy((string _) => Guid.NewGuid()))
		{
			if (list.Count >= section.ItemsCount)
			{
				break;
			}
			GIKey randomItemsByShopSection = _mergeItemInfoService.GetRandomItemsByShopSection(item2, section.ContentType, bundle);
			Item item = _itemFactoryHelper.CreateShopItem(randomItemsByShopSection, section);
			list.Add(item);
		}
		return list.ToArray();
	}
}
