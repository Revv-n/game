using System;
using System.Linq;
using StripClub.Extensions;
using StripClub.Messenger;
using StripClub.Model.Cards;
using UniRx;

namespace GreenT.HornyScapes.Messenger;

public class DialoguesTracker
{
	private readonly IMessengerManager _messengerManager;

	private readonly CardsCollection _cardsCollection;

	public DialoguesTracker(IMessengerManager messengerManager, CardsCollection cardsCollection)
	{
		_messengerManager = messengerManager;
		_cardsCollection = cardsCollection;
	}

	public bool IsNeededrarity(ChatMessage chatMessage, Rarity rarity)
	{
		Dialogue dialogue = _messengerManager.GetDialogues().FirstOrDefault((Dialogue dialogue) => dialogue.ID == chatMessage.DialogueID);
		if (dialogue != null)
		{
			Conversation conversation = _messengerManager.GetConversations().FirstOrDefault((Conversation conversation) => conversation.ID == dialogue.ConversationID);
			if (conversation != null && conversation.ParticipantIDs.Any())
			{
				return _cardsCollection.Collection.Any((ICard card) => card.ID == conversation.ParticipantIDs[0] && card.Rarity == rarity);
			}
		}
		return false;
	}

	public bool IsNeededDialogue(ChatMessage chatMessage, int girlId)
	{
		Dialogue dialogue = _messengerManager.GetDialogues().FirstOrDefault((Dialogue dialogue) => dialogue.ID == chatMessage.DialogueID);
		if (dialogue != null)
		{
			return _messengerManager.GetConversations().FirstOrDefault((Conversation conversation) => conversation.ID == dialogue.ConversationID && conversation.ParticipantIDs.Contains(girlId)) != null;
		}
		return false;
	}

	public int TryGetAllReceivedPhotoes(int girlId)
	{
		return _messengerManager.GetConversations().FirstOrDefault((Conversation conversation) => conversation.ParticipantIDs.Contains(girlId))?.CurrentMedia ?? 0;
	}

	public int TryGetAllSpecificGirlResponse(int conversationId)
	{
		return _messengerManager.GetConversations().FirstOrDefault((Conversation conversation) => conversation.ID == conversationId)?.Dialogues.Count((Dialogue p) => p.IsComplete) ?? 0;
	}

	public int TryGetAllDeliveredPlayerMessageCount(int conversationId)
	{
		return _messengerManager.GetConversations().FirstOrDefault((Conversation conversation) => conversation.ID == conversationId)?.Dialogues.Sum((Dialogue dialogue) => dialogue.Messages.Count((ChatMessage message) => message.MessageType == MessageType.Player && message.State.Contains(ChatMessage.MessageState.Delivered))) ?? 0;
	}

	public string GetCharacterNameKey(int conversationId)
	{
		Conversation conversation2 = _messengerManager.GetConversations().FirstOrDefault((Conversation conversation) => conversation.ID == conversationId);
		if (conversation2 != null)
		{
			return conversation2.NameKey;
		}
		return "";
	}

	public int GetConversationId(int girlId)
	{
		return _messengerManager.GetConversations().First((Conversation x) => x.ParticipantIDs.Contains(girlId)).ID;
	}

	public IObservable<ChatMessage> GetConcreteNewPhotoStream()
	{
		return Observable.Select<MessengerUpdateArgs, ChatMessage>(Observable.Where<MessengerUpdateArgs>(_messengerManager.OnUpdate, (Func<MessengerUpdateArgs, bool>)((MessengerUpdateArgs args) => args.Message != null && args.Message is CharacterChatMessage characterChatMessage && characterChatMessage.Attachements.Any() && characterChatMessage.State == ChatMessage.MessageState.Delivered)), (Func<MessengerUpdateArgs, ChatMessage>)((MessengerUpdateArgs args) => args.Message));
	}

	public IObservable<bool> ObserveDeliveredPlayerMessages(int conversationId)
	{
		return Observable.Select<ChatMessage, bool>(Observable.Where<ChatMessage>(Observable.Where<ChatMessage>(Observable.Select<MessengerUpdateArgs, ChatMessage>(Observable.Where<MessengerUpdateArgs>(_messengerManager.OnUpdate, (Func<MessengerUpdateArgs, bool>)((MessengerUpdateArgs args) => args.Message != null)), (Func<MessengerUpdateArgs, ChatMessage>)((MessengerUpdateArgs args) => args.Message)), (Func<ChatMessage, bool>)((ChatMessage message) => message.MessageType == MessageType.Player)), (Func<ChatMessage, bool>)((ChatMessage message) => ValidateConversation(conversationId, message))), (Func<ChatMessage, bool>)((ChatMessage chatMessage) => chatMessage.State == ChatMessage.MessageState.Delivered));
	}

	private bool ValidateConversation(int conversationId, ChatMessage chatMessage)
	{
		Dialogue dialogue = _messengerManager.GetDialogues().FirstOrDefault((Dialogue x) => x.ID == chatMessage.DialogueID);
		if (dialogue != null)
		{
			return dialogue.ConversationID == conversationId;
		}
		return false;
	}

	public IObservable<bool> GetSpecificGirlResponseStream(int conversationId)
	{
		return Observable.Select<Dialogue, bool>(Observable.Where<Dialogue>(Observable.Where<Dialogue>(Observable.Select<MessengerUpdateArgs, Dialogue>(_messengerManager.OnUpdate, (Func<MessengerUpdateArgs, Dialogue>)((MessengerUpdateArgs args) => args.Dialogue)), (Func<Dialogue, bool>)((Dialogue dialogue) => dialogue != null)), (Func<Dialogue, bool>)((Dialogue dialogue) => dialogue.ConversationID == conversationId)), (Func<Dialogue, bool>)((Dialogue dialogue) => dialogue.IsComplete));
	}

	public IObservable<bool> GetAnyNewPhotoStream()
	{
		return Observable.Select<MessengerUpdateArgs, bool>(_messengerManager.OnUpdate, (Func<MessengerUpdateArgs, bool>)((MessengerUpdateArgs args) => args.Message != null && args.Message is CharacterChatMessage characterChatMessage && characterChatMessage.Attachements.Any() && characterChatMessage.State == ChatMessage.MessageState.Delivered));
	}
}
