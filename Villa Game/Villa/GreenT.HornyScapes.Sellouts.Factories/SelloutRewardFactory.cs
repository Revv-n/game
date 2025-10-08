using System;
using System.Collections.Generic;
using GreenT.Extensions;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Presents.Factories;
using GreenT.HornyScapes.Sellouts.Mappers;
using GreenT.Types;
using Zenject;

namespace GreenT.HornyScapes.Sellouts.Factories;

public class SelloutRewardFactory : IFactory<SelloutRewardsMapper, string, int, int, bool, IReadOnlyList<RewardWithManyConditions>>, IFactory
{
	private readonly LinkedContentFactory _linkedContentFactory;

	private readonly PresentContentFactory _presentContentFactory;

	private readonly LinkedContentAnalyticDataFactory _analyticDataFactory;

	private readonly SelloutRewardsConditionFactory _selloutRewardsConditionFactory;

	private readonly IContentAdder _contentAdder;

	public SelloutRewardFactory(LinkedContentFactory linkedContentFactory, PresentContentFactory presentContentFactory, LinkedContentAnalyticDataFactory analyticDataFactory, SelloutRewardsConditionFactory selloutRewardsConditionFactory, IContentAdder contentAdder)
	{
		_linkedContentFactory = linkedContentFactory;
		_presentContentFactory = presentContentFactory;
		_analyticDataFactory = analyticDataFactory;
		_selloutRewardsConditionFactory = selloutRewardsConditionFactory;
		_contentAdder = contentAdder;
	}

	public IReadOnlyList<RewardWithManyConditions> Create(SelloutRewardsMapper mapper, string saveKey, int selloutId, int pointsToUnlock, bool isPremium)
	{
		if (!CheckRewardsLength(mapper))
		{
			throw new ArgumentException();
		}
		string[] array = (isPremium ? mapper.premium_id : mapper.rewards_id);
		RewType[] array2 = (isPremium ? mapper.premium_type : mapper.rewards_type);
		int[] array3 = (isPremium ? mapper.premium_qty : mapper.rewards_qty);
		int num = array.Length;
		List<RewardWithManyConditions> list = new List<RewardWithManyConditions>(num);
		for (int i = 0; i < num; i++)
		{
			string text = array[i];
			RewType rewType = array2[i];
			int quantity = array3[i];
			Selector selector = SelectorTools.CreateSelector(text);
			LinkedContentAnalyticData analyticData = _analyticDataFactory.Create(CurrencyAmplitudeAnalytic.SourceType.Sellout);
			IConditionReceivingReward[] conditions = _selloutRewardsConditionFactory.Create(selloutId, pointsToUnlock);
			RewardWithManyConditions item = new RewardWithManyConditions(content: (rewType != RewType.Resource || !text.Contains("present")) ? _linkedContentFactory.Create(rewType, selector, quantity, 0, ContentType.Main, analyticData) : _presentContentFactory.Create(text, selector as CurrencySelector, quantity, analyticData), saveKey: $"{saveKey}.{i}", conditions: conditions, contentAdder: _contentAdder, selector: selector);
			list.Add(item);
		}
		return list;
	}

	private bool CheckRewardsLength(SelloutRewardsMapper mapper)
	{
		return ExtensionMethods.CheckRewardsLength(mapper.rewards_type.Length, mapper.rewards_type, "rewards_type", mapper.rewards_id, "rewards_id", mapper.rewards_qty, "rewards_qty");
	}
}
