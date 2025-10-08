using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Analytics;

public class PixelLoadingAnalytic : BaseAnalytic
{
	private readonly GameStarter gameStarter;

	private readonly IEvent loadEvent;

	public PixelLoadingAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, GameStarter gameStarter, [Inject(Id = "Load")] IEvent loadEvent)
		: base(amplitude)
	{
		this.gameStarter = gameStarter;
		this.loadEvent = loadEvent;
	}

	public override void Track()
	{
		onNewStream.Clear();
		base.Track();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)gameStarter.IsGameReadyToStart, (Func<bool, bool>)((bool v) => v)), (Action<bool>)delegate
		{
			loadEvent.Send();
		}), (ICollection<IDisposable>)onNewStream);
	}
}
