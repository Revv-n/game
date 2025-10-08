using System;
using System.Collections.Generic;
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
		CompositeDisposable obj = messagesUpdateStream;
		if (obj != null)
		{
			obj.Dispose();
		}
		base.Set(source);
		bool flag = DisplayCondition();
		if (flag != IsActive())
		{
			Display(flag);
		}
		IConnectableObservable<Dialogue> val = Observable.Publish<Dialogue>(Observable.Merge<Dialogue>(Observable.ToObservable<Dialogue>(source.Dialogues.Where((Dialogue _dialogue) => !_dialogue.IsComplete)), new IObservable<Dialogue>[1] { Observable.Select<Conversation, Dialogue>(source.OnUpdate, (Func<Conversation, Dialogue>)((Conversation _conversation) => _conversation.Dialogues.Last())) }));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>(Observable.Select<ChatMessage, bool>(Observable.TakeUntilDisable<ChatMessage>(Observable.SelectMany<Dialogue, ChatMessage>(Observable.Merge<Dialogue>(Observable.SelectMany<Dialogue, Dialogue>((IObservable<Dialogue>)val, (Func<Dialogue, IObservable<Dialogue>>)((Dialogue _dialogue) => Observable.TakeWhile<Dialogue>(_dialogue.OnUpdate, (Func<Dialogue, bool>)((Dialogue __dialogue) => !__dialogue.IsComplete)))), new IObservable<Dialogue>[1] { (IObservable<Dialogue>)val }), (Func<Dialogue, IObservable<ChatMessage>>)UnpassedMessagesUpdate), parentObject), (Func<ChatMessage, bool>)((ChatMessage _) => DisplayCondition())), (Func<bool, bool>)((bool _display) => _display != IsActive())), (Action<bool>)Display), (ICollection<IDisposable>)messagesUpdateStream);
		DisposableExtensions.AddTo<IDisposable>(val.Connect(), (ICollection<IDisposable>)messagesUpdateStream);
	}

	private IObservable<ChatMessage> UnpassedMessagesUpdate(Dialogue dialogue)
	{
		ChatMessage[] array = dialogue.Messages.Where((ChatMessage _message) => !_message.State.Contains(ChatMessage.MessageState.Read)).ToArray();
		return Observable.Merge<ChatMessage>(Observable.Concat<ChatMessage>(Observable.ToObservable<ChatMessage>((IEnumerable<ChatMessage>)array), new IObservable<ChatMessage>[1] { Observable.Merge<ChatMessage>(array.Select((ChatMessage _message) => _message.OnUpdate)) }), new IObservable<ChatMessage>[1] { Observable.SelectMany<Dialogue, ChatMessage>(dialogue.OnUpdate, (Func<Dialogue, IObservable<ChatMessage>>)((Dialogue _) => dialogue.LastMessage.OnUpdate)) });
	}

	private void OnDestroy()
	{
		CompositeDisposable obj = messagesUpdateStream;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}
