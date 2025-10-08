using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.Tasks;
using GreenT.Types;
using Merge;
using StripClub.Model;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.MergeStore;

public class RegularItemFactory
{
	private readonly MergeItemInfoService _mergeItemInfoService;

	private readonly TaskManagerCluster _taskManagerCluster;

	private readonly ICurrencyProcessor _currencyProcessor;

	private readonly ItemFactoryHelper _itemFactoryHelper;

	private readonly Random _random = new Random();

	private CollectionController CollectionController => Controller<CollectionController>.Instance;

	public RegularItemFactory(MergeItemInfoService mergeItemInfoService, TaskManagerCluster taskManagerCluster, ICurrencyProcessor currencyProcessor, ItemFactoryHelper itemFactoryHelper)
	{
		_mergeItemInfoService = mergeItemInfoService;
		_taskManagerCluster = taskManagerCluster;
		_currencyProcessor = currencyProcessor;
		_itemFactoryHelper = itemFactoryHelper;
	}

	public Item[] Generate(StoreSection section, string bundle)
	{
		HashSet<GIKey> hashSet = new HashSet<GIKey>();
		bool haveEnergy = _currencyProcessor.IsEnough(CurrencyType.Energy, section.EnergyThreshold);
		string shopSection = section.ContentType.ToString();
		GIKey[] taskItems = GetTaskItems(section, bundle, shopSection);
		GIKey[] allOpenItems = GetAllOpenItems(section, bundle, shopSection, taskItems);
		AddItems(taskItems, hashSet, haveEnergy, section, shopSection);
		if (hashSet.Count >= section.ItemsCount)
		{
			return GetFullyCompletedCollection(section, hashSet, allOpenItems);
		}
		GIKey[] uniqItemsForCollection = GetUniqItemsForCollection(allOpenItems, hashSet);
		AddItems(uniqItemsForCollection, hashSet, haveEnergy, section, shopSection);
		return GetFullyCompletedCollection(section, hashSet, allOpenItems);
	}

	private Item[] GetFullyCompletedCollection(StoreSection section, HashSet<GIKey> generatedItems, GIKey[] unlockedItems)
	{
		if (generatedItems.Count >= section.ItemsCount)
		{
			return generatedItems.Select((GIKey giKey) => _itemFactoryHelper.CreateShopItem(giKey, section)).ToArray();
		}
		int count = section.ItemsCount - generatedItems.Count;
		IEnumerable<GIKey> second = (from key in unlockedItems
			where !generatedItems.Contains(key)
			select key into _
			orderby Guid.NewGuid()
			select _).Take(count);
		return (from giKey in generatedItems.Union(second)
			select _itemFactoryHelper.CreateShopItem(giKey, section)).ToArray();
	}

	private static GIKey[] GetUniqItemsForCollection(GIKey[] unlockedItems, HashSet<GIKey> generatedItems)
	{
		return (from key in unlockedItems
			where !generatedItems.Contains(key)
			select key into _
			orderby Guid.NewGuid()
			select _).ToArray();
	}

	private GIKey[] GetAllOpenItems(StoreSection section, string eventBundle, string shopSection, GIKey[] activeObjectives)
	{
		return (from item in CollectionController.GetAllOpened()
			where ValidateRegularObject(item, shopSection, eventBundle, section.ContentType)
			select item).Union(activeObjectives).Distinct().ToArray();
	}

	private GIKey[] GetTaskItems(StoreSection section, string eventBundle, string shopSection)
	{
		return (from objective in _taskManagerCluster[section.ContentType].ActiveObjectives.OfType<MergeItemObjective>()
			select objective.ItemKey into item
			where ValidateRegularObject(item, shopSection, eventBundle, section.ContentType)
			select item).Distinct().ToArray();
	}

	private void AddItems(GIKey[] targets, HashSet<GIKey> generatedItems, bool haveEnergy, StoreSection section, string shopSection)
	{
		List<GIKey> list = new List<GIKey>();
		HashSet<string> hashSet = new HashSet<string>();
		for (int i = 0; i < targets.Length; i++)
		{
			GIKey gIKey = targets[i];
			if (generatedItems.Count >= section.ItemsCount)
			{
				return;
			}
			if (hashSet.Contains(gIKey.Collection))
			{
				list.Add(gIKey);
				continue;
			}
			GIKey item = (haveEnergy ? SelectItemByTierProbability(gIKey, shopSection) : SelectLowerTierItem(gIKey, section.EnergyLowerTierChance, shopSection));
			generatedItems.Add(item);
			hashSet.Add(gIKey.Collection);
		}
		hashSet.Clear();
	}

	private bool ValidateRegularObject(GIKey key, string shopSection, string bundle, ContentType contentType)
	{
		GIConfig config = _mergeItemInfoService.GetConfig(key);
		if (config == null)
		{
			return false;
		}
		if (config.ContentType != contentType)
		{
			return false;
		}
		if (!config.EqualsOrEvent(bundle))
		{
			return false;
		}
		if (!config.HasModule<ModuleConfigs.MergeShop>())
		{
			return false;
		}
		if (!config.GetModule<ModuleConfigs.MergeShop>().ShopSection.Equals(shopSection))
		{
			return false;
		}
		return true;
	}

	private GIKey SelectItemByTierProbability(GIKey giKey, string shopSection)
	{
		int newTier = ClampedTier(giKey.Collection, giKey.ID - _random.Next(0, 2));
		if (!_mergeItemInfoService.TryGetSellableItemByCollectionAndTier(giKey.Collection, newTier, shopSection, out var key))
		{
			return giKey;
		}
		return key;
	}

	private GIKey SelectLowerTierItem(GIKey giKey, int sameTierChance, string shopSection)
	{
		if (_random.Next(100) < sameTierChance)
		{
			return giKey;
		}
		int newTier = ClampedTier(giKey.Collection, giKey.ID - 1);
		if (!_mergeItemInfoService.TryGetSellableItemByCollectionAndTier(giKey.Collection, newTier, shopSection, out var key))
		{
			return giKey;
		}
		return key;
	}

	private int ClampedTier(string itemCollection, int value)
	{
		return ClampedTier(itemCollection, ref value);
	}

	private int ClampedTier(string itemCollection, ref int value)
	{
		_mergeItemInfoService.GetTierRange(itemCollection, out var min, out var max);
		value = Math.Clamp(value, min, max);
		return value;
	}
}
