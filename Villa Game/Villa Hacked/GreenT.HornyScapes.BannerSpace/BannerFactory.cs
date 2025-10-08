using System;
using System.Linq;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.MiniEvents;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.BannerSpace;

public class BannerFactory
{
	private readonly LootboxCollection _lootboxCollection;

	private readonly GarantChanceManager _garantChanceManager;

	private readonly LinkedContentFactory _linkedContentFactory;

	private readonly CharacterManager _characterManager;

	public BannerFactory(LootboxCollection lootboxCollection, GarantChanceManager garantChanceManager, LinkedContentFactory linkedContentFactory, CharacterManager characterManager)
	{
		_lootboxCollection = lootboxCollection;
		_garantChanceManager = garantChanceManager;
		_linkedContentFactory = linkedContentFactory;
		_characterManager = characterManager;
	}

	public Banner Create(CreateData createData)
	{
		try
		{
			CreateCost(createData, out var price, out var price2, out var priceRebuy);
			CreateLootboxes(createData, out var mainRewardLootbox, out var legendaryRewardLootbox, out var epicRewardLootbox, out var rareRewardLootbox);
			CreateChances(createData.Chances, out var garantChance, out var mainRewardChance, out var epicRewardChance);
			CreateRewards(createData, out var legendaryRewardInfos, out var epicRewardInfos, out var rareRewardInfos);
			ContentType contentType = GetContentType(createData);
			return new Banner(createData.Info.Id, createData.Info.Source, createData.Info.BankTabId, createData.Info.BannerGroup, contentType, createData.BackgroundBundle, price, price2, priceRebuy, mainRewardLootbox, legendaryRewardLootbox, epicRewardLootbox, rareRewardLootbox, garantChance, mainRewardChance, epicRewardChance, legendaryRewardInfos, epicRewardInfos, rareRewardInfos, createData.Unlock.Locker);
		}
		catch (Exception)
		{
			return null;
		}
	}

	private static ContentType GetContentType(CreateData createData)
	{
		if (createData.Type != ConfigContentType.Event)
		{
			return ContentType.Main;
		}
		return ContentType.Event;
	}

	private void CreateCost(CreateData createData, out Price<int> price1, out Price<int> price10, out Price<int> priceRebuy)
	{
		price1 = null;
		price10 = null;
		priceRebuy = null;
		BannerCostData cost = createData.Cost;
		Selector selector = SelectorTools.CreateSelector(cost.BuyResource);
		Selector selector2 = SelectorTools.CreateSelector(cost.RebuyResource);
		if (selector is CurrencySelector currencySelector)
		{
			price1 = new Price<int>(cost.PriceX1, currencySelector.Currency, currencySelector.Identificator);
			price10 = new Price<int>(cost.PriceX10, currencySelector.Currency, currencySelector.Identificator);
		}
		if (selector2 is CurrencySelector currencySelector2)
		{
			priceRebuy = new Price<int>(cost.RebuyCost1x, currencySelector2.Currency, currencySelector2.Identificator);
		}
	}

	private void CreateLootboxes(CreateData createData, out Lootbox mainRewardLootbox, out Lootbox legendaryRewardLootbox, out Lootbox epicRewardLootbox, out Lootbox rareRewardLootbox)
	{
		BannerMainRewardData mainReward = createData.MainReward;
		mainRewardLootbox = _lootboxCollection.Get(mainReward.MainRewardId);
		legendaryRewardLootbox = _lootboxCollection.Get(mainReward.LegendaryRewardId);
		epicRewardLootbox = _lootboxCollection.Get(mainReward.EpicRewardId);
		rareRewardLootbox = _lootboxCollection.Get(mainReward.RareRewardId);
	}

	private void CreateChances(BannerChancesData chances, out Chance garantChance, out Chance mainRewardChance, out Chance epicRewardChance)
	{
		GarantChance garantChance2 = _garantChanceManager.Collection.FirstOrDefault((GarantChance gc) => gc.ID == chances.GarantId);
		garantChance = new Chance(garantChance2);
		mainRewardChance = new Chance(chances.MainRewardChances);
		epicRewardChance = new Chance(chances.EpicRewardChances);
	}

	private void CreateRewards(CreateData createData, out RewardInfo[] legendaryRewardInfos, out RewardInfo[] epicRewardInfos, out RewardInfo[] rareRewardInfos)
	{
		CreateLegendaryRewards(createData, out legendaryRewardInfos);
		CreateEpicRewards(createData, out epicRewardInfos);
		CreateRareRewards(createData, out rareRewardInfos);
	}

	private void CreateLegendaryRewards(CreateData createData, out RewardInfo[] legendaryRewardInfos)
	{
		LegendaryBlockData legendaryBlock = createData.LegendaryBlock;
		legendaryRewardInfos = new RewardInfo[legendaryBlock.RewardsValueQty.Length];
		for (int i = 0; i < legendaryBlock.RewardsValueQty.Length; i++)
		{
			RewardInfo rewardInfo = CreateReward(Rarity.Legendary, legendaryBlock.RewardsType[i], legendaryBlock.RewardsValueQty[i], legendaryBlock.RewardsChances[i], legendaryBlock.NewMarkers[i], legendaryBlock.MainMarkers[i]);
			legendaryRewardInfos[i] = rewardInfo;
		}
	}

	private void CreateEpicRewards(CreateData createData, out RewardInfo[] epicRewardInfos)
	{
		EpicBlockData epicBlock = createData.EpicBlock;
		epicRewardInfos = new RewardInfo[epicBlock.RewardsValueQty.Length];
		for (int i = 0; i < epicBlock.RewardsValueQty.Length; i++)
		{
			RewardInfo rewardInfo = CreateReward(Rarity.Epic, epicBlock.RewardsType[i], epicBlock.RewardsValueQty[i], epicBlock.RewardsChances[i]);
			epicRewardInfos[i] = rewardInfo;
		}
	}

	private void CreateRareRewards(CreateData createData, out RewardInfo[] rareRewardInfos)
	{
		RareBlockData rareBlock = createData.RareBlock;
		rareRewardInfos = new RewardInfo[rareBlock.RewardsValueQty.Length];
		for (int i = 0; i < rareBlock.RewardsValueQty.Length; i++)
		{
			RewardInfo rewardInfo = CreateReward(Rarity.Rare, rareBlock.RewardsType[i], rareBlock.RewardsValueQty[i], rareBlock.RewardsChances[i]);
			rareRewardInfos[i] = rewardInfo;
		}
	}

	private RewardInfo CreateReward(Rarity rarity, RewType rewType, string rewardValueQty, int rewardChance, int isNew, int isMain)
	{
		return CreateReward(rarity, rewType, rewardValueQty, rewardChance, isNew == 1, isMain == 1);
	}

	private RewardInfo CreateReward(Rarity rarity, RewType rewType, string rewardValueQty, int rewardChance, bool isNew = false, bool isMain = false)
	{
		ContentType contentType = ContentType.Main;
		string[] array = rewardValueQty.Split(':', StringSplitOptions.None);
		Selector selector = SelectorTools.CreateSelector(array[0]);
		int quantity = int.Parse(array[1]);
		TryChangeContentType(rewType, selector, ref contentType);
		LinkedContent linkedContent = _linkedContentFactory.Create(rewType, selector, quantity, 0, contentType, new LinkedContentAnalyticData(CurrencyAmplitudeAnalytic.SourceType.Banner));
		return new RewardInfo(rarity, linkedContent, rewardChance, isNew, isMain);
	}

	private void TryChangeContentType(RewType rewardType, Selector selector, ref ContentType contentType)
	{
		if (rewardType != 0)
		{
			return;
		}
		SelectorByID selectorByID = selector as SelectorByID;
		if (selectorByID != null)
		{
			ICharacter character = _characterManager.Collection.FirstOrDefault((ICharacter _character) => _character.ID == selectorByID.ID);
			if (character != null)
			{
				contentType = character.ContentType;
			}
		}
	}
}
