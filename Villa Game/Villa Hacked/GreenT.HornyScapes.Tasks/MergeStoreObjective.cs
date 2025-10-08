using System;
using GreenT.HornyScapes.MergeStore;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public abstract class MergeStoreObjective<T> : GainObjective where T : IValuableSignal
{
	private readonly SignalBus _signalBus;

	private IDisposable _anyPromoteStream;

	protected MergeStoreObjective(Func<Sprite> iconProvider, SavableObjectiveData data, SignalBus signalBus)
		: base(iconProvider, data)
	{
		_signalBus = signalBus;
	}

	public override void Track()
	{
		base.Track();
		_anyPromoteStream?.Dispose();
		_anyPromoteStream = ObservableExtensions.Subscribe<T>(_signalBus.GetStream<T>(), (Action<T>)delegate(T signal)
		{
			AddProgress(signal.Value);
		});
	}

	private void AddProgress(int signalValue)
	{
		Data.Progress += signalValue;
		onUpdate.OnNext((IObjective)this);
	}

	public override void OnRewardTask()
	{
		base.OnRewardTask();
		_anyPromoteStream?.Dispose();
	}
}
