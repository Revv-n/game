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
		gameStarter.IsGameReadyToStart.Where((bool v) => v).Subscribe(delegate
		{
			loadEvent.Send();
		}).AddTo(onNewStream);
	}
}
