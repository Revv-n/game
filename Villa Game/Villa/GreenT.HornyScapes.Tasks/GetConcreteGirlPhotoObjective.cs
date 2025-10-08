using System;
using GreenT.HornyScapes.Messenger;
using StripClub.Messenger;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class GetConcreteGirlPhotoObjective : GainObjective, IConversationObjective
{
	public readonly int GirlID;

	protected readonly DialoguesTracker _dialoguesTracker;

	protected IDisposable _photoStream;

	public int ConversationId => _dialoguesTracker.GetConversationId(GirlID);

	public GetConcreteGirlPhotoObjective(Func<Sprite> iconProvider, SavableObjectiveData data, DialoguesTracker dialoguesTracker, int girlId)
		: base(iconProvider, data)
	{
		GirlID = girlId;
		_dialoguesTracker = dialoguesTracker;
	}

	public override void Track()
	{
		SetProgress();
		base.Track();
		_photoStream?.Dispose();
		_photoStream = (from message in _dialoguesTracker.GetConcreteNewPhotoStream()
			where _dialoguesTracker.IsNeededDialogue(message, GirlID)
			select message).Subscribe(delegate
		{
			AddProgress();
		});
	}

	public override void OnRewardTask()
	{
		base.OnRewardTask();
		_photoStream?.Dispose();
	}

	private void SetProgress()
	{
		Data.Progress = _dialoguesTracker.TryGetAllReceivedPhotoes(GirlID);
		onUpdate.OnNext(this);
	}

	protected void AddProgress()
	{
		Data.Progress++;
		onUpdate.OnNext(this);
	}
}
