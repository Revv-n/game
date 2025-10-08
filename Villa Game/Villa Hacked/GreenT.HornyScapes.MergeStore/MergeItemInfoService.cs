using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.Lootboxes;
using GreenT.Types;
using Merge;
using StripClub.Extensions;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.MergeStore;

public class MergeItemInfoService
{
	private GameItemConfigManager _gameItemConfigManager;

	[Inject]
	private void Construct(GameItemConfigManager gameItemConfigManager)
	{
		_gameItemConfigManager = gameItemConfigManager;
	}

	public int GetItemPrice(GIKey itemKey)
	{
		return GetMergeShopModule(itemKey).Price;
	}

	public CurrencyType GetItemPriceResource(GIKey itemKey)
	{
		SelectorTools.GetResourceEnumValueByConfigKey(GetMergeShopModule(itemKey).PriceResource, out var currency);
		return currency;
	}

	public bool TryGetSellableItemByCollectionAndTier(string itemCollection, int newTier, string sectionFilter, out GIKey key)
	{
		key = default(GIKey);
		GIConfig gIConfig = _gameItemConfigManager.GetCollection(itemCollection).FirstOrDefault((GIConfig x) => x.Level == newTier);
		if (gIConfig == null)
		{
			return false;
		}
		if (gIConfig.TryGetModule<ModuleConfigs.MergeShop>(out var result) && result.ShopSection.Equals(sectionFilter))
		{
			key = gIConfig.Key;
		}
		return key != default(GIKey);
	}

	public int GetItemSaleByRarityIndex(GIKey itemItemKey, int discountRarityIndex)
	{
		return GetMergeShopModule(itemItemKey).Sale[discountRarityIndex];
	}

	public GIKey GetRandomItemsByShopSection(string shopSection, ContentType contentType, string eventBundle)
	{
		ModuleConfigs.MergeShop result;
		return (from config in _gameItemConfigManager.Collection
			where config.ContentType.Equals(contentType) && config.EqualsOrEvent(eventBundle)
			where config.TryGetModule<ModuleConfigs.MergeShop>(out result) && result.ShopSection.Equals(shopSection)
			select config.Key into key
			group key by key.Collection).Random().Random();
	}

	public List<string> GetAllPremiumItemShopSections(ContentType contentType, string eventBundle)
	{
		return (from config in _gameItemConfigManager.Collection
			where config.ContentType == contentType && config.EqualsOrEvent(eventBundle)
			where config.HasModule<ModuleConfigs.MergeShop>()
			select config.GetModule<ModuleConfigs.MergeShop>().ShopSection).Where(IsPremium).Distinct().ToList();
	}

	private ModuleConfigs.MergeShop GetMergeShopModule(GIKey itemKey)
	{
		return GetConfig(itemKey)?.GetModule<ModuleConfigs.MergeShop>();
	}

	public void GetTierRange(string itemCollection, out int min, out int max)
	{
		min = _gameItemConfigManager.GetCollection(itemCollection).Min((GIConfig x) => x.Level);
		max = _gameItemConfigManager.GetCollection(itemCollection).Max((GIConfig x) => x.Level);
	}

	private bool IsPremium(string shopSection)
	{
		if (!shopSection.Equals("Main"))
		{
			return !shopSection.Equals("Event");
		}
		return false;
	}

	public GIConfig GetConfig(GIKey key)
	{
		if (!_gameItemConfigManager.TryGetConfig(key, out var giConfig))
		{
			return null;
		}
		return giConfig;
	}

	public ContentType GetContentType(GIKey key)
	{
		return GetConfig(key).ContentType;
	}
}
