using GreenT.HornyScapes.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Analytics;

public class NutakuLoadingAnalytic : AnalyticWindow<LoadingWindow>
{
	[SerializeField]
	private Button _playButton;

	private GameStartStats _gameStartStats;

	private UserStats _userStats;

	[Inject]
	public void Init(GameStartStats gameStartStats, UserStats userStats)
	{
		_gameStartStats = gameStartStats;
		_userStats = userStats;
	}

	private void OnValidate()
	{
		if (!_playButton)
		{
			Debug.LogError(GetType().Name + ": put PlayButton!", this);
		}
	}

	private void Awake()
	{
		if (PlatformHelper.IsNutakuMonetization() && _userStats.InGameTime.Minutes < 20)
		{
			_playButton.onClick.AddListener(SendFirstStartEvent);
		}
	}

	private void SendFirstStartEvent()
	{
		if (_gameStartStats.CurrentState == GameStartStats.State.FirstPlay)
		{
			FirstStartAmplitudeEvent analyticsEvent = new FirstStartAmplitudeEvent();
			amplitude.AddEvent(analyticsEvent);
			_gameStartStats.CurrentState = GameStartStats.State.HasPlayed;
		}
	}
}
