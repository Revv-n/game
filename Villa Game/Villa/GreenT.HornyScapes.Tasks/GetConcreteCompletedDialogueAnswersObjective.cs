using System;
using GreenT.HornyScapes.Messenger;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class GetConcreteCompletedDialogueAnswersObjective : GainObjective, IConversationObjective
{
	private readonly int _conversationId;

	private readonly DialoguesTracker _dialoguesTracker;

	private IDisposable _completedStream;

	public string CharacterNameKey { get; private set; }

	public int ConversationId => _conversationId;

	public GetConcreteCompletedDialogueAnswersObjective(Func<Sprite> iconProvider, SavableObjectiveData data, DialoguesTracker dialoguesTracker, int conversationId)
		: base(iconProvider, data)
	{
		_conversationId = conversationId;
		_dialoguesTracker = dialoguesTracker;
	}

	public override void Track()
	{
		SetProgress();
		base.Track();
		_completedStream?.Dispose();
		_completedStream = (from dialogueIsComplete in _dialoguesTracker.ObserveDeliveredPlayerMessages(_conversationId)
			where dialogueIsComplete
			select dialogueIsComplete).Subscribe(delegate
		{
			AddProgress();
		});
	}

	public override void OnRewardTask()
	{
		base.OnRewardTask();
		_completedStream?.Dispose();
	}

	private void SetProgress()
	{
		CharacterNameKey = _dialoguesTracker.GetCharacterNameKey(_conversationId);
		Data.Progress = _dialoguesTracker.TryGetAllDeliveredPlayerMessageCount(_conversationId);
		onUpdate.OnNext(this);
	}

	private void AddProgress()
	{
		Data.Progress++;
		onUpdate.OnNext(this);
	}
}
