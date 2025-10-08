using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Saves;
using StripClub.Model;
using StripClub.UI;
using UniRx;

namespace GreenT.HornyScapes;

public sealed class RatingController : IController, IDisposable
{
	private const int ZERO = 0;

	private readonly RatingService _ratingService;

	private readonly RatingRewardService _ratingRewardService;

	private readonly SaveController _saveController;

	private RatingData _targetRatingData;

	private LeaderboardResponse _leaderboardResponse;

	private ICurrenciesActionContainer _currenciesActionContainer;

	private IDisposable _addStreamDisposable;

	private IDisposable _ratingTimeTrackDisposable;

	private IDisposable _addCurrencyRequestDisposable;

	private IDisposable _localGetLeaderboardDisposable;

	private bool _isCurrencyTrackable;

	public GenericTimer Timer { get; set; }

	public int EventId => _targetRatingData.EventID;

	public int CalendarId => _targetRatingData.CalendarID;

	public int RatingId => _targetRatingData.TargetRating.ID;

	public event Action<int, int> LeaderboardUpdated;

	public event Action<LeaderboardResponse> LeaderboardResponseUpdated;

	public RatingController(ICurrenciesActionContainer currenciesActionContainer, RatingService ratingService, RatingData targetRatingData, SaveController saveController, RatingRewardService ratingRewardService, bool isCurrencyTrackable = true)
	{
		_currenciesActionContainer = currenciesActionContainer;
		_ratingService = ratingService;
		_targetRatingData = targetRatingData;
		_saveController = saveController;
		_ratingRewardService = ratingRewardService;
		_isCurrencyTrackable = isCurrencyTrackable;
	}

	public void Initialize()
	{
		TryRegister();
	}

	public void Dispose()
	{
		_addStreamDisposable?.Dispose();
		_ratingTimeTrackDisposable?.Dispose();
		_addCurrencyRequestDisposable?.Dispose();
		_localGetLeaderboardDisposable?.Dispose();
	}

	public void RefreshSavables()
	{
		_targetRatingData.Initialize();
	}

	public void OnClaimReward()
	{
		_ratingRewardService.ClaimReward(_targetRatingData, EventId, CalendarId, RatingId);
	}

	public IDisposable GetLeaderboard(Action onError, Action onSuccess)
	{
		return _ratingService.GetLeaderboard(_targetRatingData, onError).Subscribe(delegate(LeaderboardResponse response)
		{
			if (response != null)
			{
				_leaderboardResponse = response;
				this.LeaderboardResponseUpdated?.Invoke(_leaderboardResponse);
				UpdateTargetRatingData();
				onSuccess();
			}
		});
	}

	public ScoreboardOpponentInfo TryGetOpponentInfoByID(int id)
	{
		if (_leaderboardResponse == null)
		{
			return null;
		}
		List<ScoreboardOpponentInfo> list = (_targetRatingData.IsGlobal ? _leaderboardResponse.global.leaderboard : _leaderboardResponse.group.leaderboard);
		if (id < list.Count)
		{
			return list[id];
		}
		return null;
	}

	public ScoreboardPlayerInfo TryGetPlayerInfo()
	{
		if (_leaderboardResponse == null)
		{
			return null;
		}
		if (!_targetRatingData.IsGlobal)
		{
			return _leaderboardResponse.group.player;
		}
		return _leaderboardResponse.global.player;
	}

	public int TryGetPlayersAmount()
	{
		if (_leaderboardResponse == null)
		{
			return 0;
		}
		if (!_targetRatingData.IsGlobal)
		{
			return _leaderboardResponse.group.leaderboard.Count;
		}
		return _leaderboardResponse.global.leaderboard.Count;
	}

	public int TryGetGlobalPlace()
	{
		if (_leaderboardResponse == null)
		{
			return 0;
		}
		if (!_leaderboardResponse.global.leaderboard.Any())
		{
			return 0;
		}
		return _leaderboardResponse.global.player.position;
	}

	public int TryGetGroupPlace()
	{
		if (_leaderboardResponse == null)
		{
			return 0;
		}
		if (!_leaderboardResponse.group.leaderboard.Any())
		{
			return 0;
		}
		return _leaderboardResponse.group.player.position;
	}

	public void OnLeaderboardResponseUpdated(LeaderboardResponse leaderboardResponse)
	{
		_leaderboardResponse = leaderboardResponse;
		UpdateTargetRatingData();
	}

	private void UpdateTargetRatingData()
	{
		_targetRatingData.IsCheating = (_targetRatingData.IsGlobal ? _leaderboardResponse.global.player.is_cheating : _leaderboardResponse.group.player.is_cheating);
		_targetRatingData.Place = (_targetRatingData.IsGlobal ? _leaderboardResponse.global.player.position : _leaderboardResponse.group.player.position);
		if (!_targetRatingData.IsRewarded)
		{
			LootboxLinkedContent lootboxLinkedContent = (_targetRatingData.IsGlobal ? GetConcreteReward(_leaderboardResponse.global, _targetRatingData.PlayerPower, _targetRatingData) : GetConcreteReward(_leaderboardResponse.group, _targetRatingData.PlayerPower, _targetRatingData));
			string rewardId = _targetRatingData.RewardId;
			string rewardId2 = $"{0}";
			if (lootboxLinkedContent != null && lootboxLinkedContent.Lootbox != null)
			{
				rewardId2 = $"{lootboxLinkedContent.Lootbox.ID}";
			}
			_targetRatingData.RewardId = rewardId2;
			if (lootboxLinkedContent == null || rewardId != _targetRatingData.RewardId)
			{
				_saveController.SaveToLocal();
				_saveController.SaveToServer();
			}
		}
	}

	private void TryRegister()
	{
		if (string.IsNullOrEmpty(_targetRatingData.AuthorizationToken))
		{
			_ratingService.TryRegister(_targetRatingData).Subscribe(delegate(RegistrationResponse response)
			{
				if (response != null)
				{
					_targetRatingData.AuthorizationToken = response.token;
				}
				TrackCurrency();
				TrackTime();
			});
		}
		else
		{
			TrackCurrency();
			TrackTime();
		}
	}

	private void TrackTime()
	{
		_ratingTimeTrackDisposable?.Dispose();
		if (Timer.TimeLeft <= TimeSpan.Zero)
		{
			_targetRatingData.IsFinished = true;
			return;
		}
		_ratingTimeTrackDisposable = Timer.OnTimeIsUp.Subscribe(delegate
		{
			_targetRatingData.IsFinished = true;
		});
	}

	private void TrackCurrency()
	{
		if (_isCurrencyTrackable)
		{
			_addStreamDisposable?.Dispose();
			_addStreamDisposable = _currenciesActionContainer.OnAdd().Subscribe(OnAddCurrency);
		}
	}

	private void OnAddCurrency(int amount)
	{
		if (!_targetRatingData.IsFinished)
		{
			_addCurrencyRequestDisposable?.Dispose();
			_addCurrencyRequestDisposable = _ratingService.AddScores(CurrencyAmplitudeAnalytic.Source[_currenciesActionContainer.LastSourceType], _targetRatingData.AuthorizationToken, amount).Subscribe(delegate
			{
				_localGetLeaderboardDisposable?.Dispose();
				_localGetLeaderboardDisposable = GetLeaderboard(null, OnLeaderboardUpdated);
			});
		}
	}

	private void OnLeaderboardUpdated()
	{
		this.LeaderboardUpdated?.Invoke(TryGetGlobalPlace(), TryGetGroupPlace());
	}

	private LootboxLinkedContent GetConcreteReward(Leaderboard leaderboard, float playerPower, RatingData ratingData)
	{
		ratingData.TargetRating.TryGetLootboxRewardForLevel(playerPower, leaderboard.player.position, out var reward);
		return reward;
	}
}
