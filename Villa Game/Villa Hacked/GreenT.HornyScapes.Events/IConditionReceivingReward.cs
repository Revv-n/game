using System;
using UniRx;

namespace GreenT.HornyScapes.Events;

public interface IConditionReceivingReward : IDisposable
{
	Type Type { get; }

	IReadOnlyReactiveProperty<ConditionState> State { get; }

	string ConditionText { get; }

	bool Validate();

	void Activate();

	void Reset();
}
