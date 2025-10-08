using StripClub.Model.Shop.Data;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class LotPurchasedSaveEvent : SaveEvent
{
	private SignalBus signalBus;

	[Inject]
	public void Init(SignalBus signalBus)
	{
		this.signalBus = signalBus;
	}

	public override void Track()
	{
		signalBus.GetStream<LotBoughtSignal>().Subscribe(delegate
		{
			Save();
		}).AddTo(saveStreams);
	}
}
