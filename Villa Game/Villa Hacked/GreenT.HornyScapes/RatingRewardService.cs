using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Content;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes;

public sealed class RatingRewardService : IDisposable
{
	private readonly ContentAdder _contentAdder;

	private readonly RatingManager _ratingManager;

	private readonly RatingDataManager _ratingDataManager;

	private readonly RatingService _ratingService;

	private readonly RatingAnalytic _ratingAnalytic;

	private readonly CompositeDisposable _leaderboardUpdateDisposable;

	public RatingRewardService(ContentAdder contentAdder, RatingManager ratingManager, RatingDataManager ratingDataManager, RatingService ratingService, RatingAnalytic ratingAnalytic)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		_contentAdder = contentAdder;
		_ratingManager = ratingManager;
		_ratingDataManager = ratingDataManager;
		_ratingService = ratingService;
		_ratingAnalytic = ratingAnalytic;
		_leaderboardUpdateDisposable = new CompositeDisposable();
	}

	public void Dispose()
	{
		CompositeDisposable leaderboardUpdateDisposable = _leaderboardUpdateDisposable;
		if (leaderboardUpdateDisposable != null)
		{
			leaderboardUpdateDisposable.Clear();
		}
		CompositeDisposable leaderboardUpdateDisposable2 = _leaderboardUpdateDisposable;
		if (leaderboardUpdateDisposable2 != null)
		{
			leaderboardUpdateDisposable2.Dispose();
		}
	}

	public void ClaimReward(RatingData ratingData, int eventId, int calendarId, int ratingId)
	{
		if (ratingData.RewardId != null)
		{
			AutoClaimReward(ratingData.PlayerPower, ratingData.Place, ratingData.TargetRating, eventId, calendarId, ratingId);
			ratingData.RewardId = null;
			ratingData.IsRewarded = true;
		}
	}

	public void TryAutoClaimReward(int eventId, int calendarId, int ratingId)
	{
		RatingData ratingData = _ratingDataManager.TryGetRatingData(eventId, calendarId, ratingId);
		if (ratingData.IsRewarded || ratingData.IsCheating)
		{
			return;
		}
		Rating rating = _ratingManager.GetRatingInfo(ratingId);
		_ = ratingData.PlayerPower;
		bool isGlobal = ratingData.IsGlobal;
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<LeaderboardResponse>(_ratingService.GetLeaderboard(ratingData, delegate
		{
			AutoClaimReward(ratingData.PlayerPower, ratingData.Place, ratingData.TargetRating, eventId, calendarId, ratingId);
		}), (Action<LeaderboardResponse>)delegate(LeaderboardResponse result)
		{
			if (result == null)
			{
				AutoClaimReward(ratingData.PlayerPower, ratingData.Place, ratingData.TargetRating, eventId, calendarId, ratingId);
			}
			else if (!(isGlobal ? result.global.player.is_cheating : result.group.player.is_cheating))
			{
				AutoClaimReward(ratingData.PlayerPower, isGlobal ? result.global.player.position : result.group.player.position, rating, eventId, calendarId, ratingId);
			}
		}), (ICollection<IDisposable>)_leaderboardUpdateDisposable);
	}

	private void AutoClaimReward(float playerPower, int playerPlace, Rating rating, int eventId, int calendarId, int ratingId)
	{
		if (rating.TryGetRewardForLevel(playerPower, playerPlace, out var rewards))
		{
			LinkedContent content = PackRewards(rewards);
			_contentAdder.AddContent(content);
			_ratingAnalytic.SendRewardReceivedEvent(calendarId, eventId, playerPlace, ratingId);
		}
	}

	private LinkedContent PackRewards(IEnumerable<LinkedContent> rewards)
	{
		LinkedContent linkedContent = null;
		foreach (LinkedContent reward in rewards)
		{
			if (linkedContent == null)
			{
				linkedContent = reward;
			}
			else
			{
				linkedContent.Insert(reward);
			}
		}
		return linkedContent;
	}
}
