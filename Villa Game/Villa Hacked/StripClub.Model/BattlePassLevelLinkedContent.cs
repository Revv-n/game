using System;
using System.Linq;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Lootboxes;
using GreenT.Types;
using Merge.Meta.RoomObjects;
using StripClub.Model.Cards;
using StripClub.Model.Shop;
using UnityEngine;

namespace StripClub.Model;

public class BattlePassLevelLinkedContent : LinkedContent
{
	[Serializable]
	public class BattlePassLevelMap : Map
	{
		[SerializeField]
		private int quantity;

		[SerializeField]
		private CompositeIdentificator compositeIdentificator;

		public int Quantity => quantity;

		public CurrencyType Type => CurrencyType.BP;

		public CompositeIdentificator CompositeIdentificator => compositeIdentificator;

		public BattlePassLevelMap(BattlePassLevelLinkedContent source, int count, CompositeIdentificator compositeIdentificator)
			: base(source)
		{
			quantity = count;
			this.compositeIdentificator = compositeIdentificator;
		}
	}

	private readonly GreenT.HornyScapes.GameSettings gameSettings;

	public readonly int Quantity;

	private readonly ICurrencyProcessor currencyProcessor;

	private readonly CurrencyAnalyticProcessingService battlePassAnalytic;

	private readonly BattlePassStateService _stateService;

	private readonly BattlePassProvider _provider;

	private readonly BattlePassSettingsProvider _settingsProvider;

	private readonly BattlePassMapperProvider _mapperProvider;

	private RewSettings RewPlaceholder => gameSettings.RewPlaceholder[RewType.Level];

	public override Type Type => typeof(BattlePassLevelLinkedContent);

	public BattlePassLevelLinkedContent(GreenT.HornyScapes.GameSettings gameSettings, int quantity, ICurrencyProcessor currencyProcessor, CurrencyAnalyticProcessingService battlePassAnalytic, LinkedContentAnalyticData analyticData, BattlePassStateService stateService, BattlePassProvider provider, BattlePassSettingsProvider settingsProvider, BattlePassMapperProvider mapperProvider, LinkedContent next = null)
		: base(analyticData, next)
	{
		Quantity = quantity;
		this.gameSettings = gameSettings;
		this.currencyProcessor = currencyProcessor;
		this.battlePassAnalytic = battlePassAnalytic;
		_stateService = stateService;
		_provider = provider;
		_settingsProvider = settingsProvider;
		_mapperProvider = mapperProvider;
	}

	public override Sprite GetIcon()
	{
		if (Quantity <= 0)
		{
			return RewPlaceholder.LevelRewardBaseIcon;
		}
		return RewPlaceholder.LevelRewardExpensiveIcon;
	}

	public override Sprite GetProgressBarIcon()
	{
		return GetIcon();
	}

	public override Rarity GetRarity()
	{
		return Rarity.Common;
	}

	public override string GetName()
	{
		return RewPlaceholder.PremiumLocalizationKey;
	}

	public override string GetDescription()
	{
		return Quantity.ToString();
	}

	public override Map GetMap()
	{
		return new BattlePassLevelMap(this, GetCount(), default(CompositeIdentificator));
	}

	public override LinkedContent Clone()
	{
		return new BattlePassLevelLinkedContent(gameSettings, Quantity, currencyProcessor, battlePassAnalytic, AnalyticData, _stateService, _provider, _settingsProvider, _mapperProvider, base.CloneOfNext);
	}

	public override void AddCurrentToPlayer()
	{
		bool flag = false;
		foreach (BattlePass item in _settingsProvider.Collection)
		{
			BattlePassStartData startData = item.Data.StartData;
			if (startData == null || (!startData.IsPurchaseComplete && !startData.IsPurchaseStarted))
			{
				continue;
			}
			startData.SetPurchaseComplete(value: false);
			startData.SetPurchaseStarted(value: false);
			flag = true;
			startData.SetPremiumPurchased(value: true);
			string text = _mapperProvider.GetEventMapper(item.ID).bp_resource;
			if (string.IsNullOrEmpty(text))
			{
				text = "bp_points";
			}
			int lastUnlockedLevel = item.FreeRewardContainer.GetLastUnlockedLevel();
			foreach (IGrouping<int, RewardWithManyConditions> item2 in item.PremiumRewardContainer.RewardLookup)
			{
				if (item2.Key > lastUnlockedLevel)
				{
					break;
				}
				foreach (RewardWithManyConditions item3 in item2)
				{
					if (!item3.IsRewarded && !item3.IsComplete)
					{
						item3.ForceSetState(EntityStatus.Complete);
					}
				}
			}
			SelectorTools.GetResourceEnumValueByConfigKey(text, out var currency);
			int pointsToLevelUp = item.GetPointsToLevelUp(currencyProcessor.GetCount(currency), Quantity);
			currencyProcessor.TryAdd(currency, pointsToLevelUp, AnalyticData.SourceType);
			battlePassAnalytic.SendCurrencyEvent(this);
		}
		if (!flag)
		{
			Debug.LogError("BattlePassLevelLinkedContent Attempt to add value without BattlePass");
		}
		base.AddCurrentToPlayer();
	}

	private int GetCount()
	{
		if (!_stateService.HaveActiveBattlePass())
		{
			return 0;
		}
		return _provider.CalendarChangeProperty.Value.Item2.GetPointsToLevelUp(currencyProcessor.GetCount(CurrencyType.BP), Quantity);
	}

	public override string ToString()
	{
		return Quantity.ToString();
	}
}
