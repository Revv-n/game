using System;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Lootboxes;
using StripClub.Model.Cards;
using UnityEngine;

namespace StripClub.Model;

public class LootboxLinkedContent : LinkedContent
{
	[Serializable]
	public class LootboxMap : Map
	{
		[SerializeField]
		private int id;

		public int Id => id;

		public LootboxMap(LootboxLinkedContent source)
			: base(source)
		{
			id = source.Lootbox.ID;
		}
	}

	private readonly ILootboxOpener lootboxOpener;

	private string rarityLocalizationKey = "ui.lootbox.rarity.";

	public Lootbox Lootbox { get; }

	public override Type Type => typeof(LootboxLinkedContent);

	public LootboxLinkedContent(Lootbox lootbox, ILootboxOpener lootboxOpener, LinkedContentAnalyticData analyticData, LinkedContent next = null)
		: base(analyticData, next)
	{
		Lootbox = lootbox;
		this.lootboxOpener = lootboxOpener;
	}

	public override string GetDescription()
	{
		return "1";
	}

	public override Sprite GetIcon()
	{
		return Lootbox.Icon;
	}

	public override bool TryGetAlternativeIcon(out Sprite sprite)
	{
		sprite = null;
		if (Lootbox.AlternativeIcon == null)
		{
			return false;
		}
		sprite = Lootbox.AlternativeIcon;
		return sprite != null;
	}

	public override Sprite GetProgressBarIcon()
	{
		return null;
	}

	public override Rarity GetRarity()
	{
		return Lootbox.Rarity;
	}

	public override string ToString()
	{
		return "Lootbox " + Lootbox.ID + " " + Lootbox.Rarity;
	}

	public override Map GetMap()
	{
		return new LootboxMap(this);
	}

	public override void AddCurrentToPlayer()
	{
		lootboxOpener.Open(Lootbox, AnalyticData.SourceType);
		base.AddCurrentToPlayer();
	}

	public override string GetName()
	{
		return rarityLocalizationKey + Lootbox.Name.ToLower();
	}

	public override LinkedContent Clone()
	{
		return new LootboxLinkedContent(Lootbox, lootboxOpener, AnalyticData, base.CloneOfNext);
	}

	public void SetSource(CurrencyAmplitudeAnalytic.SourceType sourceType)
	{
		AnalyticData = new LinkedContentAnalyticData(sourceType);
		Lootbox.SetSource(sourceType);
	}
}
