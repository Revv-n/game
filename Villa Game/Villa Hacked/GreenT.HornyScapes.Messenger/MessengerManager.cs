using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using StripClub.Messenger;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.Messenger;

public class MessengerManager : IMessengerManager, ICollectionSetter<ChatMessage>, ICollectionSetter<Dialogue>, ICollectionSetter<Conversation>, ICollectionSetter<DialogueLocker>, IDisposable
{
	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	private readonly Subject<MessengerUpdateArgs> _onUpdate = new Subject<MessengerUpdateArgs>();

	private readonly ISaver _saver;

	private readonly List<ChatMessage> _messages = new List<ChatMessage>();

	private readonly List<Dialogue> _dialogues = new List<Dialogue>();

	private readonly List<Conversation> _conversations = new List<Conversation>();

	private readonly List<DialogueLocker> _dialogueLockers = new List<DialogueLocker>();

	private readonly List<int> _brokenDialogueIDs = new List<int>();

	public IObservable<MessengerUpdateArgs> OnUpdate => Observable.AsObservable<MessengerUpdateArgs>((IObservable<MessengerUpdateArgs>)_onUpdate);

	public IEnumerable<Conversation> GetConversations()
	{
		return _conversations.AsEnumerable();
	}

	public IEnumerable<Dialogue> GetDialogues()
	{
		return _dialogues.AsEnumerable();
	}

	public IEnumerable<ChatMessage> GetMessages()
	{
		return _messages.AsEnumerable();
	}

	public IEnumerable<DialogueLocker> GetDialogueLockers()
	{
		return _dialogueLockers.AsEnumerable();
	}

	public MessengerManager(ISaver saver)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_saver = saver;
	}

	public void AddBrokenDialogue(int id)
	{
		if (!_brokenDialogueIDs.Contains(id))
		{
			_brokenDialogueIDs.Add(id);
		}
	}

	public bool TryGetConversation(int id, out Conversation conversation)
	{
		conversation = _conversations.FirstOrDefault((Conversation item) => item.ID == id);
		return conversation != null;
	}

	public void Add(params ChatMessage[] messages)
	{
		foreach (ChatMessage message in messages)
		{
			AddMessage(message);
		}
	}

	private void AddMessage(ChatMessage message)
	{
		AddNewChatMessage(message);
		_dialogues.First((Dialogue dialogue) => dialogue.ID == message.DialogueID).Add(message);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<MessengerUpdateArgs>(Observable.Select<ChatMessage, MessengerUpdateArgs>(message.OnUpdate, (Func<ChatMessage, MessengerUpdateArgs>)((ChatMessage chatMessage) => new MessengerUpdateArgs(null, null, chatMessage))), (Action<MessengerUpdateArgs>)_onUpdate.OnNext), (ICollection<IDisposable>)_disposables);
	}

	private void AddNewChatMessage(ChatMessage message)
	{
		if (_brokenDialogueIDs.Contains(message.DialogueID))
		{
			_saver.Delete(message);
		}
		_saver.Add(message);
		_messages.Add(message);
	}

	public void Add(params Dialogue[] dialogues)
	{
		_dialogues.AddRange(dialogues);
		foreach (IGrouping<int, Dialogue> item in from dialogue in dialogues
			group dialogue by dialogue.ConversationID)
		{
			if (TryGetConversation(item.Key, out var conversation))
			{
				conversation.AddRange(item.AsEnumerable());
			}
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<MessengerUpdateArgs>(Observable.Select<Dialogue, MessengerUpdateArgs>(Observable.Merge<Dialogue>(dialogues.Select((Dialogue dialogue) => dialogue.OnUpdate)), (Func<Dialogue, MessengerUpdateArgs>)((Dialogue dialogue) => new MessengerUpdateArgs(null, dialogue))), (Action<MessengerUpdateArgs>)_onUpdate.OnNext), (ICollection<IDisposable>)_disposables);
	}

	public void Add(params Conversation[] conversations)
	{
		_conversations.AddRange(conversations);
		foreach (Conversation conversation in conversations)
		{
			IEnumerable<Dialogue> enumerable = _dialogues.Where((Dialogue dialogue) => dialogue.ConversationID == conversation.ID);
			if (enumerable.Any())
			{
				conversation.AddRange(enumerable);
				MessengerUpdateArgs messengerUpdateArgs = new MessengerUpdateArgs(conversation);
				_onUpdate.OnNext(messengerUpdateArgs);
			}
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<MessengerUpdateArgs>(Observable.Select<Conversation, MessengerUpdateArgs>(Observable.Merge<Conversation>(conversations.Select((Conversation item) => item.OnUpdate)), (Func<Conversation, MessengerUpdateArgs>)((Conversation item) => new MessengerUpdateArgs(item))), (Action<MessengerUpdateArgs>)_onUpdate.OnNext), (ICollection<IDisposable>)_disposables);
	}

	public void Add(params DialogueLocker[] obj)
	{
		_dialogueLockers.AddRange(obj);
	}

	public void Dispose()
	{
		_disposables.Dispose();
		ClearInnerSubscriptions();
	}

	private void ClearInnerSubscriptions()
	{
		foreach (ChatMessage message in _messages)
		{
			_saver.Remove(message);
			message.Dispose();
		}
		foreach (Dialogue dialogue in _dialogues)
		{
			dialogue.Dispose();
		}
		foreach (Conversation conversation in _conversations)
		{
			conversation.Dispose();
		}
	}

	public void Purge()
	{
		_disposables.Clear();
		ClearInnerSubscriptions();
		_messages.Clear();
		_dialogues.Clear();
		_conversations.Clear();
	}
}
