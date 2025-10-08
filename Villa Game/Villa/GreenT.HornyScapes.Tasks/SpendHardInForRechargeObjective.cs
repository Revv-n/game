using System;
using GreenT.HornyScapes.MergeStore;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class SpendHardInForRechargeObjective : MergeStoreObjective<SpendHardForRechargeSignal>
{
	public SpendHardInForRechargeObjective(Func<Sprite> iconProvider, SavableObjectiveData data, SignalBus signalBus)
		: base(iconProvider, data, signalBus)
	{
	}
}
