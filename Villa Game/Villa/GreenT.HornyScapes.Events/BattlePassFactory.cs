using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.AssetBundles;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Lootboxes;
using GreenT.Types;
using ModestTree;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class BattlePassFactory : IFactory<BattlePassMapper, BattlePassBundleData, BattlePassCategory, BattlePass>, IFactory
{
	private readonly LinkedContentAnalyticDataFactory _analyticDataFactory;

	private readonly LinkedContentFactory _linkedFactory;

	private readonly ICurrencyProcessor _currencyProcessor;

	private readonly IClock _clock;

	private readonly IContentAdder _contentAdder;

	private readonly LotManager _lotManager;

	private readonly AssetProvider _assetProvider;

	private readonly IFactory<UnlockType, string, LockerSourceType, ILocker> _lockerFactory;

	public BattlePassFactory(LinkedContentAnalyticDataFactory analyticDataFactory, LinkedContentFactory linkedFactory, ICurrencyProcessor currencyProcessor, IClock clock, IContentAdder contentAdder, IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory, LotManager lotManager, AssetProvider assetProvider)
	{
		_analyticDataFactory = analyticDataFactory;
		_linkedFactory = linkedFactory;
		_currencyProcessor = currencyProcessor;
		_clock = clock;
		_contentAdder = contentAdder;
		_lockerFactory = lockerFactory;
		_lotManager = lotManager;
		_assetProvider = assetProvider;
	}

	public BattlePass Create(BattlePassMapper mapper, BattlePassBundleData bundle, BattlePassCategory battlePassCategory)
	{
		ReactiveProperty<bool> unlockProperty = new ReactiveProperty<bool>();
		CheckRewardPlacement(mapper);
		ILocker premiumPurchasedLocker = CreateLocker(mapper);
		CreateLevelPrice(mapper, out var progressPrice);
		string bp_resource = mapper.bp_resource;
		CurrencyType currencyType = GetCurrencyType(bp_resource);
		List<(int, RewardWithManyConditions)> first = CreateMergedRewards(unlockProperty, mapper, bundle, battlePassCategory, progressPrice, currencyType).ToList();
		List<(int, RewardWithManyConditions)> freeRewards = (from tuple in first.Concat(CreateRewardsFree(unlockProperty, mapper, bundle, battlePassCategory, progressPrice, currencyType))
			orderby tuple.Item1
			select tuple).ToList();
		List<(int, RewardWithManyConditions)> premiumRewards = (from tuple in first.Concat(CreateRewardsPremium(unlockProperty, mapper, bundle, battlePassCategory, progressPrice, currencyType))
			orderby tuple.Item1
			select tuple).ToList();
		string[] bundleIDs = mapper.any_lot_bought;
		IEnumerable<Lot> premiumLots = _lotManager.Collection.Where((Lot x) => bundleIDs.Contains(x.ID.ToString()));
		BattlePass.ViewSettings viewSettings = default(BattlePass.ViewSettings);
		viewSettings.AnnouncementBackground = bundle.AnnouncementBackground;
		viewSettings.AnnouncementTitleBackground = bundle.AnnouncementTitleBackground;
		viewSettings.Currency = bundle.Currency;
		viewSettings.HeaderImage = bundle.HeaderImage;
		viewSettings.LeftGirl = bundle.LeftGirl;
		viewSettings.RightGirl = bundle.RightGirl;
		viewSettings.RewardPreview = bundle.RewardPreview;
		viewSettings.StartWindowBackground = bundle.StartWindowBackground;
		viewSettings.LevelButton = bundle.ButtonSp;
		viewSettings.PremiumTrackArrow = bundle.PremiumTrackArrow;
		viewSettings.ProgressGirl = bundle.ProgressGirl;
		viewSettings.PurchaseWindow = bundle.DoublePurchaseWindow;
		viewSettings.RightReward = bundle.RightReward;
		BattlePass.ViewSettings viewSettings2 = viewSettings;
		BattlePass.ViewSettings viewSettings3 = ((mapper.bp_view != null && mapper.bp_view.Length != 0) ? SetupViewSettings(mapper.bp_view) : viewSettings2);
		return new BattlePass(mapper.bp_id, progressPrice, unlockProperty, viewSettings3, freeRewards, premiumRewards, bundle, premiumLots, premiumPurchasedLocker);
	}

	private BattlePass.ViewSettings SetupViewSettings(string[] presets)
	{
		List<Sprite> battlePassView = GetBattlePassView(presets);
		BattlePass.ViewSettings result = default(BattlePass.ViewSettings);
		result.AnnouncementBackground = battlePassView.ElementAt(0);
		result.AnnouncementTitleBackground = battlePassView.ElementAt(1);
		result.Currency = battlePassView.ElementAt(2);
		result.PurchaseWindow = battlePassView.ElementAt(3);
		result.LevelButton = battlePassView.ElementAt(4);
		result.PremiumTrackArrow = battlePassView.ElementAt(5);
		result.ProgressGirl = battlePassView.ElementAt(6);
		result.HeaderImage = battlePassView.ElementAt(7);
		result.LeftGirl = battlePassView.ElementAt(8);
		result.RightGirl = battlePassView.ElementAt(9);
		result.RewardPreview = battlePassView.ElementAt(10);
		result.StartWindowBackground = battlePassView.ElementAt(11);
		result.RightReward = battlePassView.ElementAt(12);
		return result;
	}

	private List<Sprite> GetBattlePassView(string[] presets)
	{
		List<Sprite> list = new List<Sprite>();
		for (int i = 0; i < presets.Length; i++)
		{
			string[] array = presets[i].Split(':');
			ContentSource contentSource = ((array.Length < 2) ? ContentSource.BattlePass : Enum.Parse<ContentSource>(array[0]));
			Sprite item = _assetProvider.FindBundleImageOrFake(contentSource, array[^1]);
			list.Add(item);
		}
		return list;
	}

	private void CreateLevelPrice(BattlePassMapper mapper, out Dictionary<int, int> progressPrice)
	{
		if (mapper.levels_cost_key == null || !mapper.levels_cost_key.Any())
		{
			MigrationToNewProgressCreateLevelPrice(mapper, out progressPrice);
			return;
		}
		progressPrice = new Dictionary<int, int>();
		int num = 0;
		int num2 = 0;
		progressPrice.Add(num2, num);
		for (int i = 0; i < mapper.levels_cost_value.Length; i++)
		{
			int num3 = mapper.levels_cost_value[i];
			int num4 = mapper.levels_cost_key[i][1];
			while (num2 < num4)
			{
				num += num3;
				num2++;
				progressPrice.Add(num2, num);
			}
		}
	}

	private void MigrationToNewProgressCreateLevelPrice(BattlePassMapper mapper, out Dictionary<int, int> progressPrice)
	{
		progressPrice = new Dictionary<int, int>();
		int num = Math.Max(mapper.target_levels_free.Max(), mapper.target_levels_premium.Max());
		int num2 = 0;
		int target_cost = mapper.target_cost;
		for (int i = 0; i <= num; i++)
		{
			progressPrice.Add(i, num2);
			num2 += target_cost;
		}
	}

	private ILocker CreateLocker(BattlePassMapper mapper)
	{
		string param = string.Join(",", mapper.any_lot_bought);
		return _lockerFactory.Create(UnlockType.AnyLotBought, param, LockerSourceType.BattlePass);
	}

	private IEnumerable<(int, RewardWithManyConditions)> CreateRewards(IReadOnlyReactiveProperty<bool> unlockProperty, BattlePassBundleData bundle, BattlePassCategory battlePassCategory, CurrencyType currencyType, int[] targetLevels, string[] rewId, RewType[] rewType, int[] rewQTY, int[] blockLevels, long[] blockDate, int id, Dictionary<int, int> progressPrice, bool isPremium, int bpId, int[] isLights, params int[] mergedLevels)
	{
		int num = 0;
		int num2 = 0;
		List<(int, long)> list = new List<(int, long)>(blockLevels.Length);
		List<(int, RewardWithManyConditions)> list2 = new List<(int, RewardWithManyConditions)>(targetLevels.Length);
		list.AddRange(blockLevels.Select((int t, int i) => (t: t, blockDate[i])));
		int[] array = (mergedLevels.Any() ? mergedLevels : targetLevels);
		try
		{
			for (int j = 0; j < array.Length; j++)
			{
				num2 = j;
				num = array[j];
				int i2 = (mergedLevels.Any() ? targetLevels.IndexOf(mergedLevels[j]) : j);
				int targetLevelCost = progressPrice[num];
				RewardWithManyConditions item = SetupReward(unlockProperty, bundle, battlePassCategory, currencyType, rewId, rewType, rewQTY, targetLevelCost, isPremium, bpId, isLights, num, i2, list);
				list2.Add((num, item));
			}
		}
		catch (Exception innerException)
		{
			string arg = (isPremium ? "Premium" : "Free");
			throw innerException.SendException(GetType().Name + " can't create RewardWithManyConditions. " + $"Check BattlePass ID_{id} reward {arg}_Level_{num} | " + $"{rewType[num2]}_{rewId[num2]}_{rewQTY[num2]} => " + "RewardType_RewardID_RewardQTY");
		}
		list.Clear();
		return list2;
	}

	private RewardWithManyConditions SetupReward(IReadOnlyReactiveProperty<bool> unlockProperty, BattlePassBundleData bundle, BattlePassCategory battlePassCategory, CurrencyType currencyType, string[] rewId, RewType[] rewType, int[] rewQTY, int targetLevelCost, bool isPremium, int bpId, int[] isLights, int level, int i, List<(int, long)> blockDates)
	{
		List<IConditionReceivingReward> list = new List<IConditionReceivingReward>();
		bool isLight = isLights.Contains(level);
		Selector selector = SelectorTools.CreateSelector(rewId[i]);
		CurrencyAmplitudeAnalytic.SourceType param = battlePassCategory switch
		{
			BattlePassCategory.Main => CurrencyAmplitudeAnalytic.SourceType.BattlePass, 
			BattlePassCategory.Event => CurrencyAmplitudeAnalytic.SourceType.EventBattlePass, 
			_ => throw new NotImplementedException("There is no analytics source for battlepass category: " + Enum.GetName(typeof(BattlePassCategory), battlePassCategory)), 
		};
		LinkedContentAnalyticData analyticData = _analyticDataFactory.Create(param);
		LinkedContent content = _linkedFactory.Create(rewType[i], selector, rewQTY[i], 0, ContentType.Main, analyticData);
		string saveKey = GetSaveKey(rewId, rewType, rewQTY, isPremium, level, i, bpId);
		AddCheckLevel(targetLevelCost, list, currencyType);
		TryAddCheckPremiumPurchase(bundle, unlockProperty, isPremium, list);
		TryAddCheckDateReached(blockDates, level, list, ref saveKey);
		return new RewardWithManyConditions(saveKey, content, list.ToArray(), _contentAdder, selector, isLight);
	}

	private void CheckRewardPlacement(BattlePassMapper mapper)
	{
		List<IGrouping<int, int>> list = (from level in mapper.target_levels_premium
			group level by level).ToList();
		List<IGrouping<int, int>> list2 = (from level in mapper.target_levels_free
			group level by level).ToList();
		RewType[] array = (from @group in FillGroupedProperty(list2, mapper.target_levels_free, mapper.rew_type_free)
			select @group.Key).ToArray();
		string[] array2 = (from @group in FillGroupedProperty(list2, mapper.target_levels_free, mapper.rew_id_free)
			select @group.Key).ToArray();
		int[] array3 = (from @group in FillGroupedProperty(list2, mapper.target_levels_free, mapper.rew_qty_free)
			select @group.Key).ToArray();
		RewType[] array4 = (from @group in FillGroupedProperty(list, mapper.target_levels_premium, mapper.rew_type_premium)
			select @group.Key).ToArray();
		string[] array5 = (from @group in FillGroupedProperty(list, mapper.target_levels_premium, mapper.rew_id_premium)
			select @group.Key).ToArray();
		int[] array6 = (from @group in FillGroupedProperty(list, mapper.target_levels_premium, mapper.rew_qty_premium)
			select @group.Key).ToArray();
		int[] target_levels_free = mapper.target_levels_free;
		foreach (int level in target_levels_free)
		{
			if (mapper.merged_levels.Contains(level))
			{
				int num = list2.IndexOf(list2.FirstOrDefault((IGrouping<int, int> item) => item.Key == level));
				int num2 = list.IndexOf(list.FirstOrDefault((IGrouping<int, int> item) => item.Key == level));
				IComparable[] array7 = new IComparable[3]
				{
					array2[num],
					array3[num],
					array[num]
				};
				IComparable[] premium = new IComparable[3]
				{
					array5[num2],
					array6[num2],
					array4[num2]
				};
				if (array7.Where((IComparable t, int i) => !t.Equals(premium[i])).Any())
				{
					throw new ArgumentException($"Found merged level {level} mismatch!\n" + $"Free: ID - {array7[0]} | QTY - {array7[1]} | Type - {array7[2]}\n" + $"Premium: ID - {premium[0]} | QTY - {premium[1]} | Type - {premium[2]}");
				}
			}
		}
	}

	private List<IGrouping<T, T>> FillGroupedProperty<T>(IEnumerable<IGrouping<int, int>> groupedLevel, int[] targetLevels, T[] rewData)
	{
		List<IGrouping<T, T>> list = new List<IGrouping<T, T>>();
		foreach (IGrouping<int, int> item2 in groupedLevel)
		{
			int count = targetLevels.IndexOf(item2.Key);
			IGrouping<T, T> item = (from type in rewData.Skip(count).Take(item2.Count())
				group type by type).FirstOrDefault();
			list.Add(item);
		}
		return list;
	}

	private static string GetSaveKey(string[] rewId, RewType[] rewType, int[] rewQTY, bool isPremium, int level, int i, int bpID)
	{
		string text = (isPremium ? "premium" : "free");
		return $"BP{bpID}_{text}_{level}_{rewId[i]}_{rewType[i]}_{rewQTY[i]}";
	}

	private IEnumerable<(int, RewardWithManyConditions)> CreateMergedRewards(IReadOnlyReactiveProperty<bool> unlockProperty, BattlePassMapper mapper, BattlePassBundleData bundle, BattlePassCategory battlePassCategory, Dictionary<int, int> progressPrice, CurrencyType currencyType)
	{
		if (mapper.merged_levels.IsEmpty())
		{
			return Enumerable.Empty<(int, RewardWithManyConditions)>();
		}
		return CreateRewards(unlockProperty, bundle, battlePassCategory, currencyType, mapper.target_levels_free, mapper.rew_id_free, mapper.rew_type_free, mapper.rew_qty_free, mapper.block_levels_free, mapper.date_free, mapper.bp_id, progressPrice, isPremium: false, mapper.bp_id, mapper.is_light_free, mapper.merged_levels);
	}

	private IEnumerable<(int, RewardWithManyConditions)> CreateRewardsFree(IReadOnlyReactiveProperty<bool> unlockProperty, BattlePassMapper mapper, BattlePassBundleData bundle, BattlePassCategory battlePassCategory, Dictionary<int, int> progressPrice, CurrencyType currencyType)
	{
		int[] target_levels_free = mapper.target_levels_free;
		List<int> list = new List<int>();
		List<RewType> list2 = new List<RewType>();
		List<string> list3 = new List<string>();
		List<int> list4 = new List<int>();
		for (int i = 0; i < target_levels_free.Length; i++)
		{
			if (!mapper.merged_levels.Contains(target_levels_free[i]))
			{
				list.Add(target_levels_free[i]);
				list2.Add(mapper.rew_type_free[i]);
				list3.Add(mapper.rew_id_free[i]);
				list4.Add(mapper.rew_qty_free[i]);
			}
		}
		return CreateRewards(unlockProperty, bundle, battlePassCategory, currencyType, list.ToArray(), list3.ToArray(), list2.ToArray(), list4.ToArray(), mapper.block_levels_free, mapper.date_free, mapper.bp_id, progressPrice, false, mapper.bp_id, mapper.is_light_free);
	}

	private IEnumerable<(int, RewardWithManyConditions)> CreateRewardsPremium(IReadOnlyReactiveProperty<bool> unlockProperty, BattlePassMapper mapper, BattlePassBundleData bundle, BattlePassCategory battlePassCategory, Dictionary<int, int> progressPrice, CurrencyType currencyType)
	{
		int[] target_levels_premium = mapper.target_levels_premium;
		List<int> list = new List<int>();
		List<RewType> list2 = new List<RewType>();
		List<string> list3 = new List<string>();
		List<int> list4 = new List<int>();
		for (int i = 0; i < target_levels_premium.Length; i++)
		{
			if (!mapper.merged_levels.Contains(target_levels_premium[i]))
			{
				list.Add(target_levels_premium[i]);
				list2.Add(mapper.rew_type_premium[i]);
				list3.Add(mapper.rew_id_premium[i]);
				list4.Add(mapper.rew_qty_premium[i]);
			}
		}
		return CreateRewards(unlockProperty, bundle, battlePassCategory, currencyType, list.ToArray(), list3.ToArray(), list2.ToArray(), list4.ToArray(), mapper.block_levels_premium, mapper.date_premium, mapper.bp_id, progressPrice, true, mapper.bp_id, mapper.is_light_premium);
	}

	private void AddCheckLevel(int targetCost, List<IConditionReceivingReward> conditionReceivingRewards, CurrencyType currencyType)
	{
		conditionReceivingRewards.Add(new ConditionsReceivingPoints(_currencyProcessor, currencyType, targetCost));
	}

	private void TryAddCheckPremiumPurchase(BattlePassBundleData bundle, IReadOnlyReactiveProperty<bool> unlockProperty, bool isPremium, List<IConditionReceivingReward> conditionReceivingRewards)
	{
		if (isPremium)
		{
			conditionReceivingRewards.Add(new ConditionsAchievingGoal(unlockProperty, bundle.ActivatePremium));
		}
	}

	private void TryAddCheckDateReached(List<(int, long)> blockDates, int level, List<IConditionReceivingReward> conditionReceivingRewards, ref string saveKey)
	{
		if (!blockDates.All(((int, long) tuple) => tuple.Item1 != level))
		{
			saveKey += $"_Date_{blockDates[level]}";
			conditionReceivingRewards.Add(new ConditionsReachingDate(blockDates.ElementAt(level).Item2, _clock));
		}
	}

	private CurrencyType GetCurrencyType(string battlePassResource)
	{
		CurrencyType currency = CurrencyType.BP;
		if (!string.IsNullOrEmpty(battlePassResource))
		{
			SelectorTools.GetResourceEnumValueByConfigKey(battlePassResource, out currency);
		}
		return currency;
	}
}
