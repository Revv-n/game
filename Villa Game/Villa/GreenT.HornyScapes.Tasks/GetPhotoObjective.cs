using System;
using GreenT.HornyScapes.Messenger;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class GetPhotoObjective : GainObjective
{
	protected readonly DialoguesTracker _dialoguesTracker;

	protected IDisposable _photoStream;

	public GetPhotoObjective(Func<Sprite> iconProvider, SavableObjectiveData data, DialoguesTracker dialoguesTracker)
		: base(iconProvider, data)
	{
		_dialoguesTracker = dialoguesTracker;
	}

	public override void Track()
	{
		base.Track();
		_photoStream?.Dispose();
		_photoStream = (from isGotNewPhoto in _dialoguesTracker.GetAnyNewPhotoStream()
			where isGotNewPhoto
			select isGotNewPhoto).Subscribe(delegate
		{
			AddProgress();
		});
	}

	public override void OnRewardTask()
	{
		base.OnRewardTask();
		_photoStream?.Dispose();
	}

	protected void AddProgress()
	{
		Debug.Log("ADD PROGRESS TO GET NEW PHOTO");
		Data.Progress++;
		onUpdate.OnNext(this);
	}
}
