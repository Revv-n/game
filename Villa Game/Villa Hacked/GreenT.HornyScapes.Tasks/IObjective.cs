using System;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

public interface IObjective
{
	IObservable<IObjective> OnUpdate { get; }

	bool IsComplete { get; }

	void Initialize();

	Sprite GetIcon();

	int GetProgress();

	int GetTarget();

	void Track();

	void OnRewardTask();
}
