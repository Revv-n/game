using System;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Serializable]
[Objective]
public abstract class Objective<TData> : IObjective
{
	protected Subject<IObjective> onUpdate = new Subject<IObjective>();

	public TData Data;

	public IObservable<IObjective> OnUpdate => onUpdate.AsObservable();

	public abstract bool IsComplete { get; }

	public Objective(TData data)
	{
		Data = data;
	}

	public virtual void Initialize()
	{
	}

	public abstract Sprite GetIcon();

	public abstract int GetProgress();

	public abstract int GetTarget();

	public abstract void Track();

	public abstract void OnRewardTask();
}
