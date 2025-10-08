using System;
using GreenT.HornyScapes.MergeCore;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class SpendHardForOpenBubbleObjective : MergeStoreObjective<SpendHardBubbleSignal>
{
	public SpendHardForOpenBubbleObjective(Func<Sprite> iconProvider, SavableObjectiveData data, SignalBus signalBus)
		: base(iconProvider, data, signalBus)
	{
	}
}
