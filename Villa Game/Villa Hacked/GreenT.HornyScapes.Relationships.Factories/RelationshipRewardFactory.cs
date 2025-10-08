using System.Collections.Generic;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Presents.Factories;
using GreenT.HornyScapes.Relationships.Mappers;
using GreenT.Types;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Factories;

public class RelationshipRewardFactory : IFactory<RelationshipRewardMapper, string, int, int, IReadOnlyList<RewardWithManyConditions>>, IFactory
{
	private readonly LinkedContentFactory _linkedContentFactory;

	private readonly PresentContentFactory _presentContentFactory;

	private readonly RelationshipRewardConditionFactory _relationshipRewardConditionFactory;

	private readonly IContentAdder _contentAdder;

	public RelationshipRewardFactory(LinkedContentFactory linkedContentFactory, PresentContentFactory presentContentFactory, RelationshipRewardConditionFactory rewardConditionFactory, IContentAdder contentAdder)
	{
		_linkedContentFactory = linkedContentFactory;
		_presentContentFactory = presentContentFactory;
		_relationshipRewardConditionFactory = rewardConditionFactory;
		_contentAdder = contentAdder;
	}

	public IReadOnlyList<RewardWithManyConditions> Create(RelationshipRewardMapper mapper, string saveKey, int relationShipId, int pointsToUnlock)
	{
		string[] rewards_id = mapper.rewards_id;
		int num = rewards_id.Length;
		List<RewardWithManyConditions> list = new List<RewardWithManyConditions>(num);
		for (int i = 0; i < num; i++)
		{
			Selector selector = SelectorTools.CreateSelector(rewards_id[i]);
			string text = mapper.rewards_id[i];
			RewType rewType = mapper.rewards_type[i];
			int quantity = mapper.rewards_qty[i];
			LinkedContentAnalyticData analyticData = new LinkedContentAnalyticData(CurrencyAmplitudeAnalytic.SourceType.Relationship);
			IConditionReceivingReward[] conditions = _relationshipRewardConditionFactory.Create(relationShipId, pointsToUnlock);
			list.Add(new RewardWithManyConditions(content: (rewType != RewType.Resource || !text.Contains("present")) ? _linkedContentFactory.Create(rewType, selector, quantity, 0, ContentType.Main, analyticData) : _presentContentFactory.Create(text, selector as CurrencySelector, quantity, analyticData), saveKey: $"{saveKey}.{i}", conditions: conditions, contentAdder: _contentAdder, selector: selector));
		}
		return list;
	}
}
