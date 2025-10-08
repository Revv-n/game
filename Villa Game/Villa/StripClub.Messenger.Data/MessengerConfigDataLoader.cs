using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Messenger.Data;
using StripClub.Extensions;
using StripClub.Model.Data;
using UniRx;

namespace StripClub.Messenger.Data;

public class MessengerConfigDataLoader : IMessengerDataLoader
{
	private readonly DialoguesConfigDataLoader dialogueConfigLoader;

	private readonly ConversationConfigDataLoader conversationConfigLoader;

	private readonly MessagesConfigDataLoader messageConfigLoader;

	private readonly ILoader<IEnumerable<DialogueLocker>> dialogueLockerLoader;

	private readonly MessageFactory messageFactory;

	public MessengerConfigDataLoader(DialoguesConfigDataLoader dialogueConfigLoader, ConversationConfigDataLoader conversationConfigLoader, MessagesConfigDataLoader messageConfigLoader, ILoader<IEnumerable<DialogueLocker>> dialogueLockerLoader, MessageFactory messageFactory = null)
	{
		this.dialogueConfigLoader = dialogueConfigLoader;
		this.conversationConfigLoader = conversationConfigLoader;
		this.messageConfigLoader = messageConfigLoader;
		this.dialogueLockerLoader = dialogueLockerLoader;
		this.messageFactory = messageFactory;
	}

	public IObservable<IEnumerable<Conversation>> LoadConversations(params int[] conversationIDs)
	{
		return conversationConfigLoader.Load(conversationIDs);
	}

	public IObservable<IEnumerable<Dialogue>> LoadDialogues(params int[] dialogue_ids)
	{
		return dialogueConfigLoader.Load(dialogue_ids);
	}

	public IObservable<IEnumerable<ChatMessage>> LoadMessages(params int[] dialogue_ids)
	{
		return messageConfigLoader.Load(dialogue_ids).SelectMany((IEnumerable<MessageConfigMapper> x) => x).Select(messageFactory.Create)
			.ToArray();
	}

	public IObservable<IEnumerable<ChatMessage>> LoadMessagesUntil(int dialogueID, int lastMessageNumber)
	{
		return (from _message in messageConfigLoader.Load(dialogueID.AsEnumerable()).SelectMany((IEnumerable<MessageConfigMapper> _mapper) => _mapper)
			where _message.Number <= lastMessageNumber
			select _message).Select(messageFactory.Create).ToArray();
	}

	public IObservable<ChatMessage> LoadNextMessage(int dialogueID, int currentMessageNumber)
	{
		return (from _message in (from _messages in messageConfigLoader.Load(dialogueID.AsEnumerable())
				select GetNextMessage(currentMessageNumber, _messages)).Do(delegate
			{
			})
			where _message != null
			select _message).Select(messageFactory.Create);
	}

	public IObservable<IEnumerable<DialogueLocker>> LoadDialogueLockers()
	{
		return dialogueLockerLoader.Load();
	}

	private static MessageConfigMapper GetNextMessage(int currentMessageNumber, IEnumerable<MessageConfigMapper> messages)
	{
		IEnumerable<MessageConfigMapper> source = messages.Where((MessageConfigMapper _mapper) => _mapper.Number > currentMessageNumber);
		if (!source.Any())
		{
			return null;
		}
		return source.Aggregate((MessageConfigMapper x, MessageConfigMapper y) => (x.Number >= y.Number) ? y : x);
	}

	public IObservable<IEnumerable<ChatMessage>> CreateMessage(IObservable<IEnumerable<MessageConfigMapper>> observableMapperLoading)
	{
		return observableMapperLoading.SelectMany((IEnumerable<MessageConfigMapper> x) => x).Select(messageFactory.Create).ToArray()
			.Share();
	}
}
