using System;

namespace GreenT.HornyScapes.Tasks;

public interface IGoal
{
	IObservable<IGoal> OnUpdate { get; }

	string Description { get; }

	IObjective[] Objectives { get; }

	ActionButtonType ActionButtonType { get; }

	bool IsComplete { get; }

	void Initialize();
}
