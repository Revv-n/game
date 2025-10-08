using GreenT.HornyScapes.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Analytics;

public class LoadingAnalytic : AnalyticWindow<LoadingWindow>
{
	private const string FIRST_START_KEY = "first_start_game";

	public Button PlayButton;

	private void OnValidate()
	{
		if (!PlayButton)
		{
			Debug.LogError(GetType().Name + ": put PlayButton!", this);
		}
	}

	private void Awake()
	{
		PlayButton.onClick.AddListener(SendStartEvent);
	}

	private void SendStartEvent()
	{
		if (PlayerPrefs.GetInt("first_start_game", 0) == 0)
		{
			PlayerPrefs.SetInt("first_start_game", 1);
			PlayerPrefs.Save();
		}
		else
		{
			StartAmplitudeEvent startAmplitudeEvent = new StartAmplitudeEvent();
			((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)startAmplitudeEvent);
		}
	}

	private void OnDestroy()
	{
		PlayButton.onClick.RemoveListener(SendStartEvent);
	}
}
