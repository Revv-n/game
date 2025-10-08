using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class ByTimeSaveEvent : SaveEvent
{
	[SerializeField]
	private float saveTimeInSeconds = 60f;

	private GameStarter gameStarter;

	private TimeSpan saveTime;

	[Inject]
	private void Init(GameStarter gameStarter)
	{
		this.gameStarter = gameStarter;
	}

	private void Awake()
	{
		saveTime = TimeSpan.FromSeconds(saveTimeInSeconds);
	}

	public override void Track()
	{
		gameStarter.IsGameActive.First((bool _isActive) => _isActive).ContinueWith(Observable.Interval(saveTime, Scheduler.MainThreadIgnoreTimeScale).Repeat()).Subscribe(delegate
		{
			Save();
		})
			.AddTo(saveStreams);
	}
}
