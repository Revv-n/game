using System;
using UniRx;

namespace GreenT.HornyScapes.Events;

public abstract class BaseConditionReceivingReward<T> : IConditionReceivingReward, IDisposable where T : IConditionReceivingReward
{
	protected readonly ReactiveProperty<ConditionState> LocalState = new ReactiveProperty<ConditionState>();

	protected IDisposable SubscribeDispose;

	public Type Type => typeof(T);

	public IReadOnlyReactiveProperty<ConditionState> State => (IReadOnlyReactiveProperty<ConditionState>)(object)LocalState;

	public abstract string ConditionText { get; }

	public abstract bool Validate();

	public void Activate()
	{
		if (IsDisabled() && !CheckIfCompleted())
		{
			Subscribe();
			SetActive();
		}
	}

	protected abstract void Subscribe();

	protected abstract bool CheckIfCompleted();

	public virtual void Reset()
	{
		SubscribeDispose?.Dispose();
		LocalState.Value = ConditionState.Disabled;
	}

	public virtual void Dispose()
	{
		SubscribeDispose?.Dispose();
	}

	public bool IsCompleted()
	{
		return LocalState.Value == ConditionState.Completed;
	}

	public bool IsDisabled()
	{
		return LocalState.Value == ConditionState.Disabled;
	}

	public bool IsActive()
	{
		return LocalState.Value == ConditionState.Active;
	}

	protected void SetCompleted()
	{
		LocalState.Value = ConditionState.Completed;
	}

	protected void SetActive()
	{
		LocalState.Value = ConditionState.Active;
	}
}
