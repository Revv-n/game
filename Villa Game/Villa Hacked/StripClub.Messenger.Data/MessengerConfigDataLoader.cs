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
		return Observable.ToArray<ChatMessage>(Observable.Select<MessageConfigMapper, ChatMessage>(Observable.SelectMany<IEnumerable<MessageConfigMapper>, MessageConfigMapper>(messageConfigLoader.Load(dialogue_ids), (Func<IEnumerable<MessageConfigMapper>, IEnumerable<MessageConfigMapper>>)((IEnumerable<MessageConfigMapper> x) => x)), (Func<MessageConfigMapper, ChatMessage>)messageFactory.Create));
	}

	public IObservable<IEnumerable<ChatMessage>> LoadMessagesUntil(int dialogueID, int lastMessageNumber)
	{
		return Observable.ToArray<ChatMessage>(Observable.Select<MessageConfigMapper, ChatMessage>(Observable.Where<MessageConfigMapper>(Observable.SelectMany<IEnumerable<MessageConfigMapper>, MessageConfigMapper>(messageConfigLoader.Load(dialogueID.AsEnumerable()), (Func<IEnumerable<MessageConfigMapper>, IEnumerable<MessageConfigMapper>>)((IEnumerable<MessageConfigMapper> _mapper) => _mapper)), (Func<MessageConfigMapper, bool>)((MessageConfigMapper _message) => _message.Number <= lastMessageNumber)), (Func<MessageConfigMapper, ChatMessage>)messageFactory.Create));
	}

	public IObservable<ChatMessage> LoadNextMessage(int dialogueID, int currentMessageNumber)
	{
		return Observable.Select<MessageConfigMapper, ChatMessage>(Observable.Where<MessageConfigMapper>(Observable.Do<MessageConfigMapper>(Observable.Select<IEnumerable<MessageConfigMapper>, MessageConfigMapper>(messageConfigLoader.Load(dialogueID.AsEnumerable()), (Func<IEnumerable<MessageConfigMapper>, MessageConfigMapper>)((IEnumerable<MessageConfigMapper> _messages) => GetNextMessage(currentMessageNumber, _messages))), (Action<MessageConfigMapper>)delegate
		{
		}), (Func<MessageConfigMapper, bool>)((MessageConfigMapper _message) => _message != null)), (Func<MessageConfigMapper, ChatMessage>)messageFactory.Create);
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
		return Observable.Share<ChatMessage[]>(Observable.ToArray<ChatMessage>(Observable.Select<MessageConfigMapper, ChatMessage>(Observable.SelectMany<IEnumerable<MessageConfigMapper>, MessageConfigMapper>(observableMapperLoading, (Func<IEnumerable<MessageConfigMapper>, IEnumerable<MessageConfigMapper>>)((IEnumerable<MessageConfigMapper> x) => x)), (Func<MessageConfigMapper, ChatMessage>)messageFactory.Create)));
	}
}
