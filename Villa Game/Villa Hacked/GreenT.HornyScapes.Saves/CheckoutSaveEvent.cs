using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class CheckoutSaveEvent : SaveEvent
{
	private PlayerStats playerStats;

	[Inject]
	public void Init(PlayerStats playerStats)
	{
		this.playerStats = playerStats;
	}

	public override void Track()
	{
		Initialize();
	}

	private void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.Skip<int>((IObservable<int>)playerStats.CheckoutAttemptCount, 1), (Action<int>)delegate
		{
			Save();
		}), (ICollection<IDisposable>)saveStreams);
	}
}
