using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Relationships.Models;
using GreenT.HornyScapes.Relationships.Providers;
using Merge.Meta.RoomObjects;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Services;

public class RelationshipLevelTracker : IInitializable, IDisposable
{
	private readonly RelationshipProvider _relationshipProvider;

	private readonly GameStarter _gameStarter;

	private readonly RelationshipLevelService _relationshipLevelService;

	private IDisposable _trackStream;

	public RelationshipLevelTracker(RelationshipProvider relationshipProvider, RelationshipLevelService relationshipLevelService, GameStarter gameStarter)
	{
		_relationshipProvider = relationshipProvider;
		_gameStarter = gameStarter;
		_relationshipLevelService = relationshipLevelService;
	}

	public void Initialize()
	{
		_trackStream = _gameStarter.IsGameActive.Where((bool x) => x).SelectMany((bool _) => _relationshipProvider.Collection).SelectMany((Relationship relationship) => (from @group in SelectUncompletedRewards(relationship.Rewards)
			select from _ in EmitOnRewardCompleted(@group)
				select new
				{
					Relationship = relationship,
					RewardGroup = @group
				}).Merge())
			.Subscribe(x =>
			{
				_relationshipLevelService.LevelUp(x.Relationship, x.RewardGroup);
			});
	}

	public void Dispose()
	{
		_trackStream.Dispose();
	}

	private IReadOnlyList<IReadOnlyList<RewardWithManyConditions>> SelectUncompletedRewards(IReadOnlyList<IReadOnlyList<RewardWithManyConditions>> rewardsList)
	{
		return rewardsList.Where((IReadOnlyList<RewardWithManyConditions> rewards) => rewards.All(delegate(RewardWithManyConditions reward)
		{
			EntityStatus value = reward.State.Value;
			return value == EntityStatus.Blocked || value == EntityStatus.InProgress;
		})).ToList();
	}

	private IObservable<IReadOnlyList<RewardWithManyConditions>> EmitOnRewardCompleted(IReadOnlyList<RewardWithManyConditions> rewardWithManyConditions)
	{
		return from state in rewardWithManyConditions.FirstOrDefault().State
			where state == EntityStatus.Complete
			select state into _
			select rewardWithManyConditions;
	}
}
