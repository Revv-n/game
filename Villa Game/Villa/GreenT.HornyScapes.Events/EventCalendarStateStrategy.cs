using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Content;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.Events;

public class EventCalendarStateStrategy : ICalendarStateStrategy
{
	private readonly CalendarModel context;

	private readonly IContentAdder contentAdder;

	private readonly RatingDataManager _ratingDataManager;

	private readonly RatingService _ratingService;

	private readonly EventSettingsProvider _settingsProvider;

	private readonly RatingPlayerPowerUpdateController _ratingPlayerPowerUpdateController;

	private readonly LastChanceFactory _lastChanceFactory;

	private readonly BattlePassSettingsProvider _battlePassSettingsProvider;

	private IDisposable _leaderboardUpdateDisposable;

	private EventCalendarStateStrategy(CalendarModel calendarModel, IContentAdder contentAdder, RatingDataManager ratingDataManager, RatingService ratingService, EventSettingsProvider settingsProvider, RatingPlayerPowerUpdateController ratingPlayerPowerUpdateController, LastChanceFactory lastChanceFactory, BattlePassSettingsProvider battlePassSettingsProvider)
	{
		context = calendarModel;
		this.contentAdder = contentAdder;
		_ratingDataManager = ratingDataManager;
		_ratingService = ratingService;
		_settingsProvider = settingsProvider;
		_ratingPlayerPowerUpdateController = ratingPlayerPowerUpdateController;
		_lastChanceFactory = lastChanceFactory;
		_battlePassSettingsProvider = battlePassSettingsProvider;
	}

	public bool CheckIfRewarded()
	{
		if (TryGetBattlePass(out var battlePass))
		{
			return battlePass.AllRewardContainer.Rewards.Any((RewardWithManyConditions reward) => reward.State.Value == EntityStatus.Complete);
		}
		return !context.RewardHolder.HasRewards();
	}

	public void OnInProgress()
	{
	}

	public void OnComplete()
	{
	}

	public void OnRewarded()
	{
		if (context.RewardHolder is Event { IsLaunched: not false } @event && context.LastChanceDuration != 0L)
		{
			LastChanceType lastChanceType = ((@event.BattlePassId != 0) ? LastChanceType.EventBP : ((@event.GlobalRatingId != 0 || @event.GroupRatingId != 0) ? LastChanceType.EventRating : LastChanceType.None));
			_lastChanceFactory.Create(@event.EventId, context.UniqID, context.EndTime, context.LastChanceDuration, lastChanceType);
		}
		IEnumerable<LinkedContent> rewards = GetRewards();
		if (rewards == null || rewards.Count() == 0)
		{
			return;
		}
		LinkedContent linkedContent = null;
		foreach (LinkedContent item in rewards)
		{
			if (linkedContent == null)
			{
				linkedContent = item;
			}
			else
			{
				linkedContent.Insert(item);
			}
		}
		Event event2 = _settingsProvider.GetEvent(context.BalanceId);
		if (event2.GroupRatingId != 0)
		{
			RatingData groupRatingData = _ratingDataManager.TryGetRatingData(context.BalanceId, context.UniqID, event2.GroupRatingId);
			if (groupRatingData != null)
			{
				_leaderboardUpdateDisposable?.Dispose();
				_leaderboardUpdateDisposable = _ratingService.GetLeaderboard(groupRatingData, delegate
				{
					_ratingPlayerPowerUpdateController.CalculateAdditivePoints(groupRatingData.Place);
				}).Subscribe(delegate(LeaderboardResponse result)
				{
					if (result != null && !result.global.player.is_cheating && !result.group.player.is_cheating)
					{
						_ratingPlayerPowerUpdateController.CalculateAdditivePoints(result.group.player.position);
					}
				});
			}
		}
		contentAdder.AddContent(linkedContent);
		if (!TryGetBattlePass(out var battlePass))
		{
			return;
		}
		foreach (RewardWithManyConditions item2 in battlePass.AllRewardContainer.Rewards.Where((RewardWithManyConditions reward) => reward.IsComplete))
		{
			item2.ForceSetState(EntityStatus.Rewarded);
		}
	}

	private IEnumerable<LinkedContent> GetRewards()
	{
		if (TryGetBattlePass(out var battlePass))
		{
			return from reward in battlePass.AllRewardContainer.Rewards
				where reward.IsComplete
				select reward.Content;
		}
		return context.RewardHolder.GetUncollectedRewardsContent();
	}

	private bool TryGetBattlePass(out BattlePass battlePass)
	{
		battlePass = null;
		if (context.EventMapper is EventMapper { bp_id: not 0 } eventMapper)
		{
			return _battlePassSettingsProvider.TryGetBattlePass(eventMapper.bp_id, out battlePass);
		}
		return false;
	}
}
