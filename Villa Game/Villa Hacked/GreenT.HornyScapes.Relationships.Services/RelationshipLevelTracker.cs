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
		_trackStream = ObservableExtensions.Subscribe(Observable.SelectMany(Observable.SelectMany<bool, Relationship>(Observable.Where<bool>((IObservable<bool>)_gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), (Func<bool, IEnumerable<Relationship>>)((bool _) => _relationshipProvider.Collection)), (Relationship relationship) => Observable.Merge(from @group in SelectUncompletedRewards(relationship.Rewards)
			select Observable.Select(EmitOnRewardCompleted(@group), (IReadOnlyList<RewardWithManyConditions> _) => new
			{
				Relationship = relationship,
				RewardGroup = @group
			}))), x =>
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
		return Observable.Select<EntityStatus, IReadOnlyList<RewardWithManyConditions>>(Observable.Where<EntityStatus>((IObservable<EntityStatus>)rewardWithManyConditions.FirstOrDefault().State, (Func<EntityStatus, bool>)((EntityStatus state) => state == EntityStatus.Complete)), (Func<EntityStatus, IReadOnlyList<RewardWithManyConditions>>)((EntityStatus _) => rewardWithManyConditions));
	}
}
