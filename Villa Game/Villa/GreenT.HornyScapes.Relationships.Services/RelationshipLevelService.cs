using System.Collections.Generic;
using GreenT.HornyScapes.Dates.Extensions;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Relationships.Analytics;
using GreenT.HornyScapes.Relationships.Factories;
using GreenT.HornyScapes.Relationships.Mappers;
using GreenT.HornyScapes.Relationships.Models;
using GreenT.HornyScapes.Relationships.Providers;
using UniRx;

namespace GreenT.HornyScapes.Relationships.Services;

public class RelationshipLevelService
{
	private readonly LevelUpCommandStorage _levelUpCommandStorage;

	private readonly LevelUpCommandFactory _levelUpCommandFactory;

	private readonly RelationshipMapperProvider _relationshipMapperProvider;

	private readonly RelationshipRewardMapperProvider _relationshipRewardMapperProvider;

	private readonly RelationshipAnalytic _relationshipAnalytic;

	public RelationshipLevelService(LevelUpCommandStorage levelUpCommandStorage, LevelUpCommandFactory commandFactory, RelationshipMapperProvider relationshipMapperProvider, RelationshipRewardMapperProvider relationshipRewardMapperProvider, RelationshipAnalytic relationshipAnalytic)
	{
		_levelUpCommandFactory = commandFactory;
		_levelUpCommandStorage = levelUpCommandStorage;
		_relationshipMapperProvider = relationshipMapperProvider;
		_relationshipRewardMapperProvider = relationshipRewardMapperProvider;
		_relationshipAnalytic = relationshipAnalytic;
	}

	public void LevelUp(Relationship relationship, IReadOnlyList<RewardWithManyConditions> completedRewards)
	{
		ReactiveProperty<int> relationshipLevel = relationship.RelationshipLevel;
		int value = relationshipLevel.Value + 1;
		relationshipLevel.Value = value;
		int num = relationship.Rewards.IndexOf(completedRewards);
		int num2 = num - 1;
		RelationshipMapper relationshipMapper = GetRelationshipMapper(relationship);
		int rewardId = GetRewardId(relationshipMapper, num);
		_relationshipAnalytic.SendPointsReceivedEvent(rewardId);
		if (num2 < 0)
		{
			LevelUp(relationship, num);
			return;
		}
		int status = GetStatus(relationshipMapper, num);
		int status2 = GetStatus(relationshipMapper, num2);
		if (status < status2)
		{
			LevelUp(relationship, num);
		}
	}

	private void LevelUp(Relationship relationship, int index)
	{
		LevelUpCommand entity = _levelUpCommandFactory.Create(relationship, index);
		_levelUpCommandStorage.Add(entity);
	}

	private RelationshipMapper GetRelationshipMapper(Relationship relationship)
	{
		int iD = relationship.ID;
		return _relationshipMapperProvider.Get(iD);
	}

	private int GetStatus(RelationshipMapper mapper, int index)
	{
		int id = mapper.rewards[index];
		return _relationshipRewardMapperProvider.Get(id).status_number;
	}

	private int GetRewardId(RelationshipMapper mapper, int index)
	{
		return mapper.rewards[index];
	}
}
