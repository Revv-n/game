using System;
using GreenT.HornyScapes.Events.Content;
using GreenT.Types;
using StripClub.Model.Shop.Data;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventLotBoughtTracker : IInitializable, IDisposable
{
	private readonly SignalBus signalBus;

	private readonly BankDataCleaner bankDataCleaner;

	private readonly ContentSelectorGroup contentSelectorGroup;

	private CompositeDisposable boughtLotStream = new CompositeDisposable();

	public EventLotBoughtTracker(SignalBus signalBus, BankDataCleaner bankDataCleaner, ContentSelectorGroup contentSelectorGroup)
	{
		this.signalBus = signalBus;
		this.bankDataCleaner = bankDataCleaner;
		this.contentSelectorGroup = contentSelectorGroup;
	}

	public void Initialize()
	{
		(from _ in signalBus.GetStream<LotBoughtSignal>()
			where contentSelectorGroup.Current == ContentType.Event
			select _).Subscribe(AddLotToManager).AddTo(boughtLotStream);
	}

	private void AddLotToManager(LotBoughtSignal signal)
	{
		bankDataCleaner.Add(signal.Lot);
	}

	public void Dispose()
	{
		boughtLotStream?.Dispose();
	}
}
