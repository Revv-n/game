using System;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventCalendarStateStrategy : ICalendarStateStrategy
{
	private readonly CalendarModel context;

	private readonly RatingDataManager _ratingDataManager;

	private readonly RatingService _ratingService;

	private readonly RatingRewardService _ratingRewardService;

	private readonly RatingManager _ratingManager;

	private IDisposable _leaderboardUpdateDisposable;

	private MiniEventCalendarStateStrategy(CalendarModel calendarModel, RatingService ratingService, RatingDataManager ratingDataManager, RatingRewardService ratingRewardService, RatingManager ratingManager)
	{
		context = calendarModel;
		_ratingService = ratingService;
		_ratingDataManager = ratingDataManager;
		_ratingRewardService = ratingRewardService;
		_ratingManager = ratingManager;
	}

	public bool CheckIfComplete()
	{
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
		MiniEventMapper miniEventMapper = context.EventMapper as MiniEventMapper;
		if (miniEventMapper.config_type == ConfigType.Rating)
		{
			_ratingRewardService.TryAutoClaimReward(context.BalanceId, context.UniqID, miniEventMapper.activity_id);
		}
	}

	public bool CheckIfRewarded()
	{
		return true;
	}
}
