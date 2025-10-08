using System;
using System.Collections.Generic;
using GreenT.Types;
using StripClub.Model.Cards;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.BannerSpace;

public class DropAnalytics
{
	[Serializable]
	public class Info
	{
		public int Step;

		public Rarity Rarity;

		public bool IsMain;

		public int MainStep;

		public int LegendaryStep;

		public int LootboxID;

		public Price<int> Price;
	}

	private readonly Banner _banner;

	private readonly Price<int> _priceStep;

	private readonly Analytics _analytics;

	private readonly ContentType _contentType;

	public readonly List<Info> Infos;

	public DropAnalytics(Banner banner, int count, Price<int> price, Analytics analytics, ContentType contentType)
	{
		_banner = banner;
		_analytics = analytics;
		_contentType = contentType;
		_priceStep = new Price<int>(price.Value / count, price.Currency, price.CompositeIdentificator);
		Infos = new List<Info>(count);
	}

	public void AddInfo(int step, Rarity rarity, bool isMain, int mainStep, int legendaryStep, int lootboxID)
	{
		Infos.Add(new Info
		{
			Step = step,
			Rarity = rarity,
			IsMain = isMain,
			MainStep = mainStep,
			LegendaryStep = legendaryStep,
			LootboxID = lootboxID,
			Price = _priceStep
		});
	}

	public void SendInfo()
	{
		foreach (Info info in Infos)
		{
			_analytics.TrackBannerReward(info.LootboxID, _banner.LegendaryChance.GarantID, info.LegendaryStep, _banner.LegendaryChance.GetValue(info.LegendaryStep), _priceStep.Currency, _priceStep.Value, _contentType);
		}
	}
}
