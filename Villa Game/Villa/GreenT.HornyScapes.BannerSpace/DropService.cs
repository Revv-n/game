using System;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Lootboxes;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.Model.Shop;
using UniRx;

namespace GreenT.HornyScapes.BannerSpace;

public class DropService
{
	private readonly SaveDataManager _saveDataManager;

	private readonly LootboxOpener _lootboxOpener;

	private readonly Analytics _analytics;

	private readonly ContentSelectorGroup _contentSelectorGroup;

	private readonly Subject<int> _onDrop = new Subject<int>();

	private readonly Subject<int> _onGrantChange = new Subject<int>();

	public IObservable<int> OnDrop => _onDrop;

	public IObservable<int> OnGrantChange => _onGrantChange;

	public DropService(SaveDataManager saveDataManager, LootboxOpener lootboxOpener, Analytics analytics, ContentSelectorGroup contentSelectorGroup)
	{
		_saveDataManager = saveDataManager;
		_lootboxOpener = lootboxOpener;
		_analytics = analytics;
		_contentSelectorGroup = contentSelectorGroup;
	}

	public void Drop(Banner banner, int count, Price<int> price)
	{
		Drop(banner, count, price, out var _);
	}

	public void Drop(Banner banner, int count, Price<int> price, out DropAnalytics dropAnalytics, bool addDrop = true)
	{
		dropAnalytics = new DropAnalytics(banner, count, price, _analytics, _contentSelectorGroup.Current);
		Lootbox cloneTarget;
		Lootbox[] drop = GetDrop(banner, count, dropAnalytics, out cloneTarget);
		if (addDrop)
		{
			LinkedContent linkedContent = drop[0].Open();
			for (int i = 1; i < count; i++)
			{
				LinkedContent content = drop[i].Open();
				linkedContent.Insert(content);
			}
			Lootbox lootbox = cloneTarget.Clone(linkedContent);
			dropAnalytics.SendInfo();
			_lootboxOpener.Open(lootbox, CurrencyAmplitudeAnalytic.SourceType.Banner);
		}
	}

	private Lootbox[] GetDrop(Banner banner, int count, DropAnalytics dropAnalytics, out Lootbox cloneTarget)
	{
		Lootbox[] array = new Lootbox[count];
		cloneTarget = null;
		Rarity rarity = Rarity.Common;
		for (int i = 0; i < count; i++)
		{
			bool isMain = false;
			bool isLegendary = false;
			StepInfo stepInfo = _saveDataManager.GetStepInfo(banner.BannerGroup);
			Rarity rarity2;
			if (IsLegendary(banner, stepInfo))
			{
				isLegendary = true;
				if (IsMain(banner, stepInfo))
				{
					isMain = true;
					array[i] = banner.MainRewardLootbox;
				}
				else
				{
					array[i] = banner.LegendaryRewardLootbox;
				}
				rarity2 = Rarity.Legendary;
			}
			else if (IsEpic(banner, stepInfo))
			{
				array[i] = banner.EpicRewardLootbox;
				rarity2 = Rarity.Epic;
			}
			else
			{
				array[i] = banner.RareRewardLootbox;
				rarity2 = Rarity.Rare;
			}
			if (rarity2 > rarity)
			{
				rarity = rarity2;
				cloneTarget = array[i];
			}
			dropAnalytics.AddInfo(i, rarity2, isMain, stepInfo.MainStep, stepInfo.LegendaryStep, array[i].ID);
			_saveDataManager.OnBannerBoth(banner.BannerGroup, isLegendary, isMain);
			_onDrop.OnNext(banner.Id);
			_onGrantChange.OnNext(banner.Id);
		}
		return array;
	}

	public int GetDifferenceBetweenSteps(Banner banner)
	{
		StepInfo stepInfo = _saveDataManager.GetStepInfo(banner.BannerGroup);
		int val = banner.LegendaryChance.MaxSteps() + 1 - stepInfo.LegendaryStep;
		return Math.Max(1, val);
	}

	private bool IsEpic(Banner banner, StepInfo stepInfo)
	{
		return banner.EpicRewardChance.IsChance(stepInfo.Step);
	}

	private bool IsMain(Banner banner, StepInfo stepInfo)
	{
		return banner.MainRewardChance.IsChance(stepInfo.MainStep);
	}

	private bool IsLegendary(Banner banner, StepInfo stepInfo)
	{
		return banner.LegendaryChance.IsChance(stepInfo.LegendaryStep);
	}
}
