using System;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public class RatingView : MonoView<RatingData>
{
	[SerializeField]
	private ScrollPool _scrollPool;

	[SerializeField]
	private RatingSelfView _playerView;

	[SerializeField]
	private RatingPlayerView[] _views;

	[SerializeField]
	private LocalizedTextMeshPro _title;

	[SerializeField]
	private int _minViewsAmount = 5;

	public RatingController RatingController;

	private User _user;

	private RatingControllerManager _ratingControllerManager;

	private IDisposable _playersViewUpdateDisposable;

	private IDisposable _leaderboardUpdateDisposable;

	private const string TITLE_ACTIVE_KEY = "ui.rating.active_{0}";

	private const string TITLE_FINISHED_KEY = "ui.rating.finished_{0}";

	private const int ERROR_PLACE = -1;

	private const int REAL_PLACE_STEP = 1;

	[Inject]
	private void Init(User user, RatingControllerManager ratingControllerManager)
	{
		_user = user;
		_ratingControllerManager = ratingControllerManager;
	}

	protected virtual void Start()
	{
		InnerInit();
	}

	private void OnDestroy()
	{
		_playersViewUpdateDisposable?.Dispose();
	}

	public void InnerInit()
	{
		if (RatingController == null)
		{
			_scrollPool.Init(_minViewsAmount);
		}
		else
		{
			int num = RatingController.TryGetPlayersAmount();
			if (num == 0)
			{
				ShowLoadingState();
			}
			_scrollPool.InitRange((num == 0) ? _minViewsAmount : num);
			_scrollPool.ForceUpdate();
		}
		_playersViewUpdateDisposable?.Dispose();
		_playersViewUpdateDisposable = _scrollPool.OnOpponentViewPositionUpdate.Subscribe(delegate(RatingPlayerView view)
		{
			if (RatingController != null)
			{
				ScoreboardOpponentInfo scoreboardOpponentInfo = RatingController.TryGetOpponentInfoByID(view.Index);
				if (scoreboardOpponentInfo != null)
				{
					base.Source.TargetRating.TryGetRewardForLevel(base.Source.PlayerPower, view.Place, out var rewards);
					view.SetupName(scoreboardOpponentInfo.player_name);
					view.SetupRewards(rewards);
					view.SetupScore(scoreboardOpponentInfo.score);
				}
				else
				{
					view.SetupName();
					view.SetupRewards(null);
					view.SetupScore();
				}
			}
		});
		_scrollPool.SetupPlayerName(_user.Nickname);
		_playerView.Init(OnClaimRewardButtonClick);
		Display(display: true);
	}

	public override void Display(bool display)
	{
		base.Display(display);
		if (base.Source != null)
		{
			TryGetController();
			OnSuccess();
			SetupCurrencyView();
			string key = string.Format(base.Source.IsFinished ? "ui.rating.finished_{0}" : "ui.rating.active_{0}", base.Source.EventID);
			_title.Init(key);
		}
	}

	public void AutoUpdate()
	{
		if (RatingController != null && base.Source != null)
		{
			_scrollPool.InitRange(_minViewsAmount);
			_scrollPool.ClearState();
			ShowLoadingState();
			_leaderboardUpdateDisposable?.Dispose();
			_leaderboardUpdateDisposable = RatingController.GetLeaderboard(OnError, OnSuccess);
		}
	}

	protected virtual void TryGetController()
	{
		RatingController = _ratingControllerManager.TryGetRatingController(base.Source);
	}

	protected virtual void OnSuccess()
	{
		UpdatePlayerView();
		UpdateOpponentViews();
		StopLoadingState();
		int num = RatingController.TryGetPlayersAmount();
		if (num == 0)
		{
			ShowLoadingState();
		}
		_scrollPool.InitRange((num == 0) ? _minViewsAmount : num);
		_scrollPool.ForceUpdate();
	}

	private void ShowLoadingState()
	{
		_playerView.ShowLoadingState();
		RatingPlayerView[] views = _views;
		for (int i = 0; i < views.Length; i++)
		{
			views[i].ShowLoadingState();
		}
	}

	private void StopLoadingState()
	{
		_playerView.StopLoadingState();
		RatingPlayerView[] views = _views;
		for (int i = 0; i < views.Length; i++)
		{
			views[i].StopLoadingState();
		}
	}

	private void SetupCurrencyView()
	{
		_playerView.SetupCurrencyIcon(base.Source.TargetRating.CurrencyType, base.Source.TargetRating.CurrencyIdentificator, base.Source.EventID);
		RatingPlayerView[] views = _views;
		for (int i = 0; i < views.Length; i++)
		{
			views[i].SetupCurrencyIcon(base.Source.TargetRating.CurrencyType, base.Source.TargetRating.CurrencyIdentificator, base.Source.EventID);
		}
	}

	private void OnError()
	{
		_playerView.StopLoadingState();
		_playerView.SetupScore();
		_playerView.SetupPlace(-1);
		RatingPlayerView[] views = _views;
		foreach (RatingPlayerView obj in views)
		{
			obj.StopLoadingState();
			obj.SetupName();
			obj.SetupScore();
		}
		_scrollPool.SetupPlayers(-1, -1);
	}

	private void UpdatePlayerView()
	{
		ScoreboardPlayerInfo scoreboardPlayerInfo = RatingController.TryGetPlayerInfo();
		if (scoreboardPlayerInfo != null)
		{
			int num = scoreboardPlayerInfo.position - 1;
			base.Source.TargetRating.TryGetRewardForLevel(base.Source.PlayerPower, scoreboardPlayerInfo.position, out var rewards);
			_playerView.SetupPlace(num);
			_playerView.SetupRewards(rewards);
			_playerView.SetupScore(scoreboardPlayerInfo.score);
			_scrollPool.SetupPlayerName(_user.Nickname);
			_scrollPool.SetupPlayers(num, num);
			if (base.Source.IsFinished && !base.Source.IsRewarded && !base.Source.IsCheating && rewards != null)
			{
				_playerView.SetupRewardState();
			}
			else
			{
				_playerView.SetupDefaultState();
			}
		}
	}

	private void UpdateOpponentViews()
	{
		int num = RatingController.TryGetPlayersAmount();
		RatingPlayerView[] views = _views;
		foreach (RatingPlayerView ratingPlayerView in views)
		{
			if (ratingPlayerView.Index < num)
			{
				base.Source.TargetRating.TryGetRewardForLevel(base.Source.PlayerPower, ratingPlayerView.Place, out var rewards);
				ScoreboardOpponentInfo scoreboardOpponentInfo = RatingController.TryGetOpponentInfoByID(ratingPlayerView.Index);
				ratingPlayerView.SetupName(scoreboardOpponentInfo.player_name);
				ratingPlayerView.SetupRewards(rewards);
				ratingPlayerView.SetupScore(scoreboardOpponentInfo.score);
			}
		}
	}

	private void OnClaimRewardButtonClick()
	{
		RatingController.OnClaimReward();
	}
}
