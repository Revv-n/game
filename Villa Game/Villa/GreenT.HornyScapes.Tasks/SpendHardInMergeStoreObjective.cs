using System;
using GreenT.HornyScapes.MergeStore;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class SpendHardInMergeStoreObjective : MergeStoreObjective<SpendHardMergeStoreSignal>
{
	public SpendHardInMergeStoreObjective(Func<Sprite> iconProvider, SavableObjectiveData data, SignalBus signalBus)
		: base(iconProvider, data, signalBus)
	{
	}
}
