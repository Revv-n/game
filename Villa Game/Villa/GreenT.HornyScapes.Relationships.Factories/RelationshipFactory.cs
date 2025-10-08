using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Relationships.Mappers;
using GreenT.HornyScapes.Relationships.Models;
using GreenT.HornyScapes.Relationships.Providers;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Factories;

public class RelationshipFactory : IFactory<RelationshipMapper, Relationship>, IFactory
{
	private const string RewardSaveKey = "relationship.{0}.reward.{1}";

	private readonly RelationshipRewardMapperProvider _relationshipRewardMapperProvider;

	private readonly RelationshipRewardFactory _relationshipRewardFactory;

	private readonly ISaver _saver;

	public RelationshipFactory(RelationshipRewardMapperProvider relationshipRewardMapperProvider, RelationshipRewardFactory relationshipRewardFactory, ISaver saver)
	{
		_relationshipRewardMapperProvider = relationshipRewardMapperProvider;
		_relationshipRewardFactory = relationshipRewardFactory;
		_saver = saver;
	}

	public Relationship Create(RelationshipMapper mapper)
	{
		List<int> list = new List<int>(8);
		IReadOnlyList<IReadOnlyList<RewardWithManyConditions>> rewards = CreateRewards(mapper, list);
		int[] rewards2 = mapper.rewards;
		Relationship relationship = new Relationship(mapper.id_relationship, list, rewards2, rewards);
		_saver.Add(relationship);
		return relationship;
	}

	private IReadOnlyList<IReadOnlyList<RewardWithManyConditions>> CreateRewards(RelationshipMapper relationshipMapper, List<int> requirementPoints)
	{
		int[] rewards = relationshipMapper.rewards;
		int id_relationship = relationshipMapper.id_relationship;
		int num = rewards.Length;
		List<IReadOnlyList<RewardWithManyConditions>> list = new List<IReadOnlyList<RewardWithManyConditions>>(num);
		for (int i = 0; i < num; i++)
		{
			RelationshipRewardMapper relationshipRewardMapper = _relationshipRewardMapperProvider.Get(rewards[i]);
			int points_to_unlock = relationshipRewardMapper.points_to_unlock;
			string saveKey = $"relationship.{id_relationship}.reward.{i}";
			IReadOnlyList<RewardWithManyConditions> item = _relationshipRewardFactory.Create(relationshipRewardMapper, saveKey, id_relationship, points_to_unlock);
			list.Add(item);
			requirementPoints.Add(points_to_unlock);
		}
		return list;
	}
}
