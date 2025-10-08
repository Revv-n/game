using System;
using System.Collections.Generic;
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		this.signalBus = signalBus;
		this.bankDataCleaner = bankDataCleaner;
		this.contentSelectorGroup = contentSelectorGroup;
	}

	public void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<LotBoughtSignal>(Observable.Where<LotBoughtSignal>(signalBus.GetStream<LotBoughtSignal>(), (Func<LotBoughtSignal, bool>)((LotBoughtSignal _) => contentSelectorGroup.Current == ContentType.Event)), (Action<LotBoughtSignal>)AddLotToManager), (ICollection<IDisposable>)boughtLotStream);
	}

	private void AddLotToManager(LotBoughtSignal signal)
	{
		bankDataCleaner.Add(signal.Lot);
	}

	public void Dispose()
	{
		CompositeDisposable obj = boughtLotStream;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}
