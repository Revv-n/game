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
		return from args in _messengerManager.OnUpdate
			where args.Message != null && args.Message is CharacterChatMessage characterChatMessage && characterChatMessage.Attachements.Any() && characterChatMessage.State == ChatMessage.MessageState.Delivered
			select args.Message;
	}

	public IObservable<bool> ObserveDeliveredPlayerMessages(int conversationId)
	{
		return from args in _messengerManager.OnUpdate
			where args.Message != null
			select args.Message into message
			where message.MessageType == MessageType.Player
			where ValidateConversation(conversationId, message)
			select message into chatMessage
			select chatMessage.State == ChatMessage.MessageState.Delivered;
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
		return from args in _messengerManager.OnUpdate
			select args.Dialogue into dialogue
			where dialogue != null
			where dialogue.ConversationID == conversationId
			select dialogue.IsComplete;
	}

	public IObservable<bool> GetAnyNewPhotoStream()
	{
		return _messengerManager.OnUpdate.Select((MessengerUpdateArgs args) => args.Message != null && args.Message is CharacterChatMessage characterChatMessage && characterChatMessage.Attachements.Any() && characterChatMessage.State == ChatMessage.MessageState.Delivered);
	}
}
