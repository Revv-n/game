using System;
using StripClub.Model.Shop.Data;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes;

public class PurchaseDataTracker : IInitializable, IDisposable
{
	private readonly PlayerPaymentsStats playerPaymentsStats;

	private readonly SignalBus signalBus;

	private IDisposable stream;

	public PurchaseDataTracker(PlayerPaymentsStats playerPaymentsStats, SignalBus signalBus)
	{
		this.playerPaymentsStats = playerPaymentsStats;
		this.signalBus = signalBus;
	}

	public void Initialize()
	{
		stream?.Dispose();
		stream = (from _signal in signalBus.GetStream<LotBoughtSignal>()
			select _signal.Lot).Subscribe(playerPaymentsStats.AddBought);
	}

	public void Dispose()
	{
		stream?.Dispose();
	}
}
