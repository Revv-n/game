using System;
using System.Collections.Generic;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.ContinueWith<bool, long>(Observable.First<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool _isActive) => _isActive)), Observable.Repeat<long>(Observable.Interval(saveTime, Scheduler.MainThreadIgnoreTimeScale))), (Action<long>)delegate
		{
			Save();
		}), (ICollection<IDisposable>)saveStreams);
	}
}
