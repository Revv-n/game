using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Extensions;
using UniRx;

namespace StripClub.Messenger;

public sealed class Dialogue : IDisposable
{
	private List<ChatMessage> messages;

	public DateTime LastTimeUpdate;

	private bool isComplete;

	private CompositeDisposable lastMessageTrackStream = new CompositeDisposable();

	private static ChatMessageComparer messageComparer = new ChatMessageComparer();

	private Subject<Dialogue> onUpdate = new Subject<Dialogue>();

	public int ID { get; private set; }

	public int TotalMessages { get; private set; }

	public int TotalPlayerMessages { get; private set; }

	public int TotalMedia { get; private set; }

	public int ConversationID { get; private set; }

	public ChatMessage LastMessage => messages.LastOrDefault();

	public int CurrentMessageAmount => messages.Count;

	public int CurrentMediaAmount => (from _message in messages.OfType<CharacterChatMessage>()
		where _message.State.Contains(ChatMessage.MessageState.Delivered)
		select _message).Sum((CharacterChatMessage x) => x.Attachements.Count());

	public float RelativeProgress
	{
		get
		{
			if (TotalMessages == 0)
			{
				return 0f;
			}
			return (float)CurrentMessageAmount / (float)TotalMessages;
		}
	}

	public bool IsLocked => !messages.Any();

	public bool IsComplete
	{
		get
		{
			return isComplete;
		}
		private set
		{
			isComplete = true;
			onUpdate.OnNext(this);
		}
	}

	public int UnreadMessagesCount => messages.Count((ChatMessage _message) => _message is CharacterChatMessage && _message.State.Contains(ChatMessage.MessageState.Delivered) && !_message.State.Contains(ChatMessage.MessageState.Read));

	public IEnumerable<ChatMessage> Messages => messages;

	public IObservable<Dialogue> OnUpdate => onUpdate;

	public Dialogue(int id, int conversationID, int totalMessages, int totalPlayerMessages, int totalMedia, List<ChatMessage> messageHistory)
		: this(id, conversationID, totalMessages, totalPlayerMessages, totalMedia)
	{
		messages = messageHistory;
		messages.Sort(messageComparer);
	}

	public Dialogue(int id, int conversationID, int totalMessages, int totalPlayerMessages, int totalMedia)
	{
		ID = id;
		ConversationID = conversationID;
		TotalMessages = totalMessages;
		TotalPlayerMessages = totalPlayerMessages;
		TotalMedia = totalMedia;
		messages = new List<ChatMessage>();
	}

	public void Initialize()
	{
		isComplete = false;
		lastMessageTrackStream.Clear();
	}

	public void Add(ChatMessage newMessage)
	{
		messages.Add(newMessage);
		messages.Sort(messageComparer);
		SetCompleteOnLastMessagePassed();
		LastTimeUpdate = ((newMessage.Time.CompareTo(LastTimeUpdate) > 0) ? newMessage.Time : LastTimeUpdate);
		onUpdate.OnNext(this);
	}

	private void SetCompleteOnLastMessagePassed()
	{
		if (RelativeProgress != 1f)
		{
			return;
		}
		if (!LastMessage.State.Contains(ChatMessage.MessageState.Passed))
		{
			LastMessage.OnUpdate.FirstOrDefault((ChatMessage _message) => _message.State.Contains(ChatMessage.MessageState.Passed)).Subscribe(delegate
			{
				IsComplete = true;
			}).AddTo(lastMessageTrackStream);
		}
		else
		{
			IsComplete = true;
		}
	}

	public void Remove(ChatMessage message)
	{
		messages.Remove(message);
		onUpdate.OnNext(this);
	}

	public void AddRange(IEnumerable<ChatMessage> newMessages)
	{
		foreach (ChatMessage newMessage in newMessages)
		{
			messages.Add(newMessage);
		}
		messages.Sort(messageComparer);
		onUpdate.OnNext(this);
		SetCompleteOnLastMessagePassed();
	}

	public override string ToString()
	{
		return base.ToString() + " ID: " + ID + " Progress: " + CurrentMessageAmount;
	}

	public void Dispose()
	{
		onUpdate?.Dispose();
		lastMessageTrackStream?.Dispose();
	}
}
