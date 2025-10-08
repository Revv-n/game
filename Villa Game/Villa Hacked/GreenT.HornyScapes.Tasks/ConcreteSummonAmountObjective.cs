using System;
using GreenT.Types;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class ConcreteSummonAmountObjective : AnySummonObjective
{
	public ConcreteSummonAmountObjective(Func<Sprite> iconProvider, SavableObjectiveData data, SignalBus signalBus)
		: base(iconProvider, data, signalBus, ContentType.Event)
	{
	}

	public override void Track()
	{
		signalStream?.Dispose();
		signalStream = ObservableExtensions.Subscribe<SummonLot>(Observable.Select<LotBoughtSignal, SummonLot>(Observable.Where<LotBoughtSignal>(signalBus.GetStream<LotBoughtSignal>(), (Func<LotBoughtSignal, bool>)((LotBoughtSignal _lot) => _lot.Lot is SummonLot summonLot && summonLot.TaskType != contentType)), (Func<LotBoughtSignal, SummonLot>)((LotBoughtSignal _lot) => _lot.Lot as SummonLot)), (Action<SummonLot>)delegate(SummonLot _lot)
		{
			AddProgress(_lot.CurrentWholesale);
		});
	}
}
