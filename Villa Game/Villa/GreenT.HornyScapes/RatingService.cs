using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.MiniEvents;
using GreenT.Net;
using StripClub.Model.Cards;
using UniRx;

namespace GreenT.HornyScapes;

public sealed class RatingService
{
	private readonly LeaderboardRequest _leaderboardRequest;

	private readonly LeaderboardAddPointsRequest _leaderboardAddPointsRequest;

	private readonly RegistrationRequest _registrationRequest;

	private readonly PowerManager _powerManager;

	private readonly CardsCollection _cardsCollection;

	private readonly TournamentPointsStorage _tournamentPointsStorage;

	public RatingService(LeaderboardRequest scoreboardRequest, LeaderboardAddPointsRequest scoreboardAddPointsRequest, RegistrationRequest registrationRequest, PowerManager powerManager, CardsCollection cardsCollection, TournamentPointsStorage tournamentPointsStorage)
	{
		_leaderboardRequest = scoreboardRequest;
		_leaderboardAddPointsRequest = scoreboardAddPointsRequest;
		_registrationRequest = registrationRequest;
		_powerManager = powerManager;
		_cardsCollection = cardsCollection;
		_tournamentPointsStorage = tournamentPointsStorage;
	}

	public IObservable<LeaderboardResponse> GetLeaderboard(RatingData ratingData, Action onError = null)
	{
		if (string.IsNullOrEmpty(ratingData.AuthorizationToken))
		{
			if (onError != null)
			{
				onError();
			}
			return Observable.Return<LeaderboardResponse>(null);
		}
		return _leaderboardRequest.GetRequest(ratingData.TargetRating.Range, ratingData.AuthorizationToken).DoOnError(delegate
		{
			if (onError != null)
			{
				onError();
			}
		});
	}

	public IObservable<Response> AddScores(string reason, string token, int amount)
	{
		return _leaderboardAddPointsRequest.Post(reason, token, amount).DoOnError(delegate
		{
		});
	}

	public IObservable<RegistrationResponse> TryRegister(RatingData targetRatingData)
	{
		TrySetPlayerPower(targetRatingData);
		return _registrationRequest.Post(targetRatingData);
	}

	private void TrySetPlayerPower(RatingData ratingData)
	{
		float lastPower = (ratingData.PlayerPower = CalculatePlayerPower());
		_tournamentPointsStorage.LastPower = lastPower;
	}

	private float CalculatePlayerPower()
	{
		PowerMapper powerInfo = _powerManager.GetPowerInfo();
		int promoteSum = GetPromoteSum(Rarity.Rare);
		int promoteSum2 = GetPromoteSum(Rarity.Epic);
		int promoteSum3 = GetPromoteSum(Rarity.Legendary);
		int promoteSum4 = GetPromoteSum(Rarity.Mythic);
		return (float)(promoteSum * powerInfo.rare_promote_coef + promoteSum2 * powerInfo.epic_promote_coef + promoteSum3 * powerInfo.legendary_promote_coef + promoteSum4 * powerInfo.mythic_promote_coef) * (1f + _tournamentPointsStorage.AdditivePoints);
	}

	private int GetPromoteSum(Rarity rarity)
	{
		int result = 0;
		IEnumerable<KeyValuePair<ICard, IPromote>> enumerable = _cardsCollection.Promote.Where((KeyValuePair<ICard, IPromote> pair) => pair.Key.Rarity == rarity);
		if (enumerable != null && enumerable.Any())
		{
			result = enumerable.Sum((KeyValuePair<ICard, IPromote> pair) => pair.Value.Level.Value);
		}
		return result;
	}
}
