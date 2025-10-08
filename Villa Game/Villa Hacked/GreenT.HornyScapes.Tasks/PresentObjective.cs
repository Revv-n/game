using System;
using GreenT.HornyScapes.Presents.Models;
using GreenT.HornyScapes.Presents.Services;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class PresentObjective : GainObjective, IDisposable
{
	protected readonly PresentsNotifier _presentsNotifier;

	protected IDisposable _presentsStream;

	public PresentObjective(Func<Sprite> iconProvider, PresentsNotifier presentsNotifier, SavableObjectiveData data)
		: base(iconProvider, data)
	{
		_presentsNotifier = presentsNotifier;
	}

	public override void Track()
	{
		base.Track();
		_presentsStream?.Dispose();
		_presentsStream = ObservableExtensions.Subscribe<Present>(_presentsNotifier.OnNotify, (Action<Present>)AddProgress);
	}

	public override void OnRewardTask()
	{
		base.OnRewardTask();
		_presentsStream?.Dispose();
	}

	public void Dispose()
	{
		_presentsStream?.Dispose();
	}

	protected virtual void AddProgress(Present present)
	{
		onUpdate.OnNext((IObjective)this);
	}
}
