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
		signalStream = (from _lot in signalBus.GetStream<LotBoughtSignal>()
			where _lot.Lot is SummonLot summonLot && summonLot.TaskType != contentType
			select _lot.Lot as SummonLot).Subscribe(delegate(SummonLot _lot)
		{
			AddProgress(_lot.CurrentWholesale);
		});
	}
}
