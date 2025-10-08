using System;
using System.Linq;
using StripClub.Extensions;
using StripClub.Messenger;
using StripClub.Messenger.UI;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Messenger.UI;

public abstract class ConversationLastMessageIndicator : ConversationView
{
	[SerializeField]
	private GameObject parentObject;

	private CompositeDisposable messagesUpdateStream = new CompositeDisposable();

	public abstract bool DisplayCondition();

	public override void Set(Conversation source)
	{
		messagesUpdateStream?.Dispose();
		base.Set(source);
		bool flag = DisplayCondition();
		if (flag != IsActive())
		{
			Display(flag);
		}
		IConnectableObservable<Dialogue> connectableObservable = source.Dialogues.Where((Dialogue _dialogue) => !_dialogue.IsComplete).ToObservable().Merge(source.OnUpdate.Select((Conversation _conversation) => _conversation.Dialogues.Last()))
			.Publish();
		(from _ in connectableObservable.SelectMany((Dialogue _dialogue) => _dialogue.OnUpdate.TakeWhile((Dialogue __dialogue) => !__dialogue.IsComplete)).Merge(connectableObservable).SelectMany((Func<Dialogue, IObservable<ChatMessage>>)UnpassedMessagesUpdate)
				.TakeUntilDisable(parentObject)
			select DisplayCondition() into _display
			where _display != IsActive()
			select _display).Subscribe(Display).AddTo(messagesUpdateStream);
		connectableObservable.Connect().AddTo(messagesUpdateStream);
	}

	private IObservable<ChatMessage> UnpassedMessagesUpdate(Dialogue dialogue)
	{
		ChatMessage[] source = dialogue.Messages.Where((ChatMessage _message) => !_message.State.Contains(ChatMessage.MessageState.Read)).ToArray();
		return source.ToObservable().Concat(source.Select((ChatMessage _message) => _message.OnUpdate).Merge()).Merge(dialogue.OnUpdate.SelectMany((Dialogue _) => dialogue.LastMessage.OnUpdate));
	}

	private void OnDestroy()
	{
		messagesUpdateStream?.Dispose();
	}
}
