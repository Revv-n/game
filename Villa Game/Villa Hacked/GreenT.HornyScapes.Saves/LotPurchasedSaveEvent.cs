using System;
using System.Collections.Generic;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<LotBoughtSignal>(signalBus.GetStream<LotBoughtSignal>(), (Action<LotBoughtSignal>)delegate
		{
			Save();
		}), (ICollection<IDisposable>)saveStreams);
	}
}
