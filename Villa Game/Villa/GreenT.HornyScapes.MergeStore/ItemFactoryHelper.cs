using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using Merge;
using StripClub.Model;

namespace GreenT.HornyScapes.MergeStore;

public class ItemFactoryHelper
{
	private readonly MergeItemInfoService _mergeItemInfoService;

	private const int Amount = 1;

	private readonly Random _random = new Random();

	public ItemFactoryHelper(MergeItemInfoService mergeItemInfoService)
	{
		_mergeItemInfoService = mergeItemInfoService;
	}

	public Item CreateShopItem(GIKey itemKey, StoreSection storeSection)
	{
		CurrencyType itemPriceResource = _mergeItemInfoService.GetItemPriceResource(itemKey);
		int itemPrice = _mergeItemInfoService.GetItemPrice(itemKey);
		ContentType contentType = _mergeItemInfoService.GetContentType(itemKey);
		return new Item(Guid.NewGuid().ToString(), itemKey, itemPrice, itemPrice, itemPriceResource, 0, 0, 1, contentType, storeSection.SaleTierDifference, storeSection.Type);
	}

	public void ShuffleItems<T>(T[] items)
	{
		for (int num = items.Length - 1; num > 0; num--)
		{
			int num2 = _random.Next(num + 1);
			int num3 = num;
			int num4 = num2;
			T val = items[num2];
			T val2 = items[num];
			items[num3] = val;
			items[num4] = val2;
		}
	}

	public int DetermineDiscountSlotsCount(IReadOnlyCollection<int> discountChances)
	{
		int num = _random.Next(100);
		int num2 = 0;
		for (int i = 0; i < discountChances.Count; i++)
		{
			num2 += discountChances.ElementAt(i);
			if (num < num2)
			{
				return i;
			}
		}
		return 0;
	}

	public void ApplyDiscountsToItems(Item[] items, int discountSlotsCount, StoreSection section)
	{
		if (discountSlotsCount <= 0 || items.Length == 0)
		{
			return;
		}
		discountSlotsCount = Math.Min(discountSlotsCount, items.Length);
		List<int> list = new List<int>();
		List<int> list2 = Enumerable.Range(0, items.Length).ToList();
		for (int i = 0; i < discountSlotsCount; i++)
		{
			if (list2.Count == 0)
			{
				break;
			}
			int index = _random.Next(list2.Count);
			list.Add(list2[index]);
			list2.RemoveAt(index);
		}
		foreach (int item2 in list)
		{
			Item item = items[item2];
			int discountRarityIndex = GetDiscountRarityIndex(section.RarityChance);
			int itemSaleByRarityIndex = _mergeItemInfoService.GetItemSaleByRarityIndex(item.ItemKey, discountRarityIndex);
			int salePrice = (int)((float)item.BasePrice * (1f - (float)itemSaleByRarityIndex / 100f));
			items[item2] = new Item(item.Id, item.ItemKey, item.BasePrice, salePrice, item.CurrencyType, itemSaleByRarityIndex, discountRarityIndex, item.Amount, item.ContentType, item.SaleTierDifference, item.Section);
		}
	}

	private int GetDiscountRarityIndex(int[] rarityChance)
	{
		int num = _random.Next(100);
		int num2 = 0;
		for (int i = 0; i < rarityChance.Length; i++)
		{
			num2 += rarityChance[i];
			if (num < num2)
			{
				return i;
			}
		}
		return 0;
	}
}
