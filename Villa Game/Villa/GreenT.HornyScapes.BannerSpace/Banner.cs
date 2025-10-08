using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Lootboxes;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.BannerSpace;

public class Banner
{
	public readonly int Id;

	public readonly string Source;

	public readonly int BankTabId;

	public readonly string BannerGroup;

	public readonly ContentType ContentType;

	public readonly BannerBackgroundBundle Background;

	public readonly Price<int> BuyPrice;

	public readonly Price<int> BuyPrice10;

	public readonly Price<int> RebuyCost;

	public readonly Lootbox MainRewardLootbox;

	public readonly Lootbox LegendaryRewardLootbox;

	public readonly Lootbox EpicRewardLootbox;

	public readonly Lootbox RareRewardLootbox;

	public readonly Chance LegendaryChance;

	public readonly Chance MainRewardChance;

	public readonly Chance EpicRewardChance;

	public readonly RewardInfo[] LegendaryRewardInfos;

	public readonly RewardInfo[] EpicRewardInfos;

	public readonly RewardInfo[] RareRewardInfos;

	public readonly ILocker Locker;

	public Banner(int id, string source, int bankTabId, string bannerGroup, ContentType contentType, BannerBackgroundBundle background, Price<int> buyPrice, Price<int> buyPrice10, Price<int> rebuyCost, Lootbox mainRewardLootbox, Lootbox legendaryRewardLootbox, Lootbox epicRewardLootbox, Lootbox rareRewardLootbox, Chance legendaryChance, Chance mainRewardChance, Chance epicRewardChance, RewardInfo[] legendaryRewardInfos, RewardInfo[] epicRewardInfos, RewardInfo[] rareRewardInfos, ILocker locker)
	{
		Id = id;
		Source = source;
		BankTabId = bankTabId;
		BannerGroup = bannerGroup;
		ContentType = contentType;
		Background = background;
		BuyPrice = buyPrice;
		BuyPrice10 = buyPrice10;
		RebuyCost = rebuyCost;
		MainRewardLootbox = mainRewardLootbox;
		LegendaryRewardLootbox = legendaryRewardLootbox;
		EpicRewardLootbox = epicRewardLootbox;
		RareRewardLootbox = rareRewardLootbox;
		LegendaryChance = legendaryChance;
		MainRewardChance = mainRewardChance;
		EpicRewardChance = epicRewardChance;
		LegendaryRewardInfos = legendaryRewardInfos;
		EpicRewardInfos = epicRewardInfos;
		RareRewardInfos = rareRewardInfos;
		Locker = locker;
	}

	public IEnumerable<LinkedContent> GetAllRewards()
	{
		return from r in LegendaryRewardInfos.Concat(EpicRewardInfos).Concat(RareRewardInfos)
			select r.LinkedContent;
	}
}
