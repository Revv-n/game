using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Lootboxes;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;

namespace StripClub.UI.Shop;

public class SingleSummonLotView : SummonLotView
{
	[SerializeField]
	private LootboxContentView regularLootboxContentView;

	[SerializeField]
	private LootboxContentView rareLootboxContentView;

	[SerializeField]
	private LootboxContentView ultraRareLootboxContentView;

	[SerializeField]
	private GameObject singleSummonTimerDescription;

	[SerializeField]
	private TimerContainer singleSummonTimerContainer;

	private const int firstLootboxIndex = 1;

	private const int secondLootboxIndex = 2;

	private const int thirdLootboxIndex = 3;

	public override void Set(Lot lot)
	{
		base.Set(lot);
		DisplayContent();
		SetTimer();
	}

	private void DisplayContent()
	{
		try
		{
			List<RandomDropSettings> randomDropSettings = GetRandomDropSettings();
			LootboxLinkedContent lootbox = GetLootbox(randomDropSettings[1]);
			LootboxLinkedContent lootbox2 = GetLootbox(randomDropSettings[2]);
			LootboxLinkedContent lootbox3 = GetLootbox(randomDropSettings[3]);
			regularLootboxContentView.SetPossibleRewardsWithLootboxRarity(lootbox);
			rareLootboxContentView.SetPossibleRewardsWithLootboxRarity(lootbox2);
			ultraRareLootboxContentView.SetPossibleRewardsWithLootboxRarity(lootbox3);
		}
		catch (Exception)
		{
		}
	}

	private List<RandomDropSettings> GetRandomDropSettings()
	{
		return ((LootboxLinkedContent)((SummonLot)base.Source).SingleRewardSettings.Reward).Lootbox.DropOptions;
	}

	private LootboxLinkedContent GetLootbox(RandomDropSettings dropOption)
	{
		return (LootboxLinkedContent)dropOption.GetDrop(CurrencyAmplitudeAnalytic.SourceType.None);
	}

	private void SetTimer()
	{
		singleSummonTimerContainer.SetEnableFromTimer(((CompositeLocker)base.Source.Locker).Lockers);
	}
}
